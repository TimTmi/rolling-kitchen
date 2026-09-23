using System.Collections.Generic;
using System.Linq;
using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class IngredientStack : Pickable
    {
        private const float MinAdjacentRotation = 45f;
        private const int MaxYawAttempts = 8;

        private readonly List<Pickable> _contents = new();

        public Pickable Root { get; private set; }

        public IEnumerable<Pickable> Contents => _contents;

        public override string GetInteractionPrompt(in InteractionContext context)
        {
            return string.Join(" + ", _contents.Select(content => content.Data.DisplayName));
        }

        public Pickable ReplaceRoot(Ingredient[] results)
        {
            Pickable oldRoot = Root;
            int rootIndex = _contents.IndexOf(oldRoot);

            Pickable newRoot = Instantiate(results[0], oldRoot.transform.parent);
            newRoot.transform.SetPositionAndRotation(oldRoot.transform.position, oldRoot.transform.rotation);

            IngredientStack stack = FormOn(newRoot);
            stack._contents.Clear();
            stack._contents.AddRange(_contents);
            stack._contents[rootIndex] = newRoot;

            foreach (Pickable content in stack._contents)
            {
                content.Stack = stack;

                if (content != newRoot)
                {
                    content.transform.SetParent(stack.transform);
                }
            }

            for (int i = 1; i < results.Length; i++)
            {
                stack.Attach(Instantiate(results[i], stack.transform), rootIndex + i);
            }

            stack.Arrange();
            Destroy(oldRoot.gameObject);
            return newRoot;
        }

        public void Replace(Pickable member, Ingredient[] results)
        {
            int index = _contents.IndexOf(member);
            if (index < 0)
            {
                return;
            }

            _contents.RemoveAt(index);
            Destroy(member.gameObject);

            foreach (Ingredient result in results)
            {
                Attach(Instantiate(result, transform), index);
                index++;
            }

            Arrange();
        }

        public static bool CanMerge(Pickable held, Pickable target)
        {
            Group heldGroup = GroupOf(held);
            Group targetGroup = GroupOf(target);

            if (heldGroup.Stack != null && heldGroup.Stack == targetGroup.Stack)
            {
                return false;
            }

            return !HasRoleConflict(heldGroup, targetGroup)
                && (FindInsertionIndex(heldGroup.Contents, targetGroup.Contents) >= 0
                    || FindInsertionIndex(targetGroup.Contents, heldGroup.Contents) >= 0);
        }

        public static void Merge(Pickable held, Pickable target, in InteractionContext context)
        {
            Group heldGroup = GroupOf(held);
            Group targetGroup = GroupOf(target);

            if (heldGroup.Role < targetGroup.Role)
            {
                if (!TryInsert(targetGroup, heldGroup))
                {
                    TryInsert(heldGroup, targetGroup);
                    context.Release();
                }

                return;
            }

            if (!TryInsert(heldGroup, targetGroup))
            {
                TryInsert(targetGroup, heldGroup);
                return;
            }

            context.Release();
        }

        public static bool CanMergeInto(Pickable contents, Pickable host)
        {
            Group contentsGroup = GroupOf(contents);
            Group hostGroup = GroupOf(host);

            return !HasRoleConflict(contentsGroup, hostGroup)
                && FindInsertionIndex(contentsGroup.Contents, hostGroup.Contents) >= 0;
        }

        public static void MergeInto(Pickable contents, Pickable host)
        {
            TryInsert(GroupOf(contents), GroupOf(host));
        }

        private static bool TryInsert(Group contentsGroup, Group hostGroup)
        {
            int index = FindInsertionIndex(contentsGroup.Contents, hostGroup.Contents);
            if (index < 0)
            {
                return false;
            }

            IngredientStack stack = hostGroup.Stack ?? FormOn(hostGroup.Contents[0]);

            foreach (Pickable content in contentsGroup.Contents)
            {
                content.Stack = stack;
                content.transform.SetParent(stack.transform);
            }

            stack._contents.InsertRange(index, contentsGroup.Contents);
            stack.RandomizeRotations(index, contentsGroup.Contents.Count);
            stack.Arrange();

            contentsGroup.Stack?.Dissolve();
            return true;
        }

        private static int FindInsertionIndex(List<Pickable> contents, List<Pickable> host)
        {
            for (int index = host.Count; index >= 0; index--)
            {
                if (IsValidSequence(host, contents, index))
                {
                    return index;
                }
            }

            return -1;
        }

        private static bool IsValidSequence(List<Pickable> host, List<Pickable> contents, int insertionIndex)
        {
            StackingRole previous = StackingRole.Container;

            for (int i = 0; i <= host.Count; i++)
            {
                if (i == insertionIndex)
                {
                    foreach (Pickable content in contents)
                    {
                        if (content.Data.StackingRole < previous)
                        {
                            return false;
                        }

                        previous = content.Data.StackingRole;
                    }
                }

                if (i < host.Count)
                {
                    if (host[i].Data.StackingRole < previous)
                    {
                        return false;
                    }

                    previous = host[i].Data.StackingRole;
                }
            }

            return true;
        }

        private static bool HasRoleConflict(Group a, Group b)
        {
            return CountRole(a, StackingRole.Container) + CountRole(b, StackingRole.Container) > 1
                || CountRole(a, StackingRole.Base) + CountRole(b, StackingRole.Base) > 1
                || CountRole(a, StackingRole.Top) + CountRole(b, StackingRole.Top) > 1;
        }

        private static int CountRole(Group group, StackingRole role)
        {
            return group.Contents.Count(content => content.Data.StackingRole == role);
        }

        private static Group GroupOf(Pickable pickable)
        {
            if (pickable is IngredientStack stack)
            {
                return new Group(stack._contents, stack);
            }

            if (pickable.Stack != null)
            {
                return new Group(pickable.Stack._contents, pickable.Stack);
            }

            return new Group(new List<Pickable> { pickable }, null);
        }

        private static IngredientStack FormOn(Pickable root)
        {
            IngredientStack stack = root.gameObject.AddComponent<IngredientStack>();
            stack.Root = root;
            stack._contents.Add(root);
            root.Stack = stack;
            return stack;
        }

        private void Attach(Pickable content, int index)
        {
            _contents.Insert(index, content);
            content.Stack = this;
            content.transform.SetParent(transform);
            RandomizeRotations(index, 1);
        }

        private void RandomizeRotations(int index, int count)
        {
            int last = index + count - 1;

            for (int i = index; i <= last; i++)
            {
                if (_contents[i].Data.StackingRole == StackingRole.Container)
                {
                    _contents[i].transform.localRotation = Quaternion.identity;
                    continue;
                }

                float? below = i > 0 ? Yaw(_contents[i - 1]) : null;
                float? above = i == last && last + 1 < _contents.Count ? Yaw(_contents[last + 1]) : null;

                _contents[i].transform.localRotation = Quaternion.Euler(0f, RandomYaw(below, above), 0f);
            }
        }

        private float Yaw(Pickable content)
        {
            return (content == Root ? transform : content.transform).localEulerAngles.y;
        }

        private static float RandomYaw(float? below, float? above)
        {
            for (int attempt = 0; attempt < MaxYawAttempts; attempt++)
            {
                float angle = Random.Range(0f, 360f);

                if (MeetsDelta(angle, below) && MeetsDelta(angle, above))
                {
                    return angle;
                }
            }

            return Random.Range(0f, 360f);
        }

        private static bool MeetsDelta(float angle, float? other)
        {
            return !other.HasValue || Mathf.Abs(Mathf.DeltaAngle(angle, other.Value)) >= MinAdjacentRotation;
        }

        private void Arrange()
        {
            float height = 0f;

            for (int i = 0; i < _contents.Count && _contents[i] != Root; i++)
            {
                height -= _contents[i].Data.StackHeight;
            }

            foreach (Pickable content in _contents)
            {
                if (content != Root)
                {
                    content.transform.localPosition = new Vector3(0f, height, 0f);
                }

                height += content.Data.StackHeight;
            }
        }

        private void Dissolve()
        {
            Destroy(this);
        }

        private readonly struct Group
        {
            public List<Pickable> Contents { get; }
            public IngredientStack Stack { get; }
            public StackingRole Role => Contents[0].Data.StackingRole;

            public Group(List<Pickable> contents, IngredientStack stack)
            {
                Contents = contents;
                Stack = stack;
            }
        }
    }
}
