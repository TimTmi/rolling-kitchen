using System.Collections.Generic;
using System.Linq;
using Features.Interaction;
using UnityEngine;

namespace Features.Pickup
{
    public class IngredientStack : Pickable
    {
        private Pickable _root;
        private readonly List<Pickable> _members = new();

        public Pickable Root => _root;

        public IEnumerable<Pickable> Contents
        {
            get
            {
                yield return _root;

                foreach (Pickable member in _members)
                {
                    yield return member;
                }
            }
        }

        public override string GetInteractionPrompt(in InteractionContext context)
        {
            return string.Join(" + ", Contents.Select(content => content.Data.DisplayName));
        }

        public Pickable ReplaceRoot(Ingredient[] results)
        {
            Pickable oldRoot = _root;

            Pickable newRoot = Instantiate(results[0], oldRoot.transform.parent);
            newRoot.transform.SetPositionAndRotation(oldRoot.transform.position, oldRoot.transform.rotation);

            IngredientStack stack = FormOn(newRoot);

            for (int i = 1; i < results.Length; i++)
            {
                stack.Append(Instantiate(results[i], stack.transform));
            }

            foreach (Pickable member in _members)
            {
                stack.Append(member);
            }

            Destroy(oldRoot.gameObject);
            return newRoot;
        }

        public void Replace(Pickable member, Ingredient[] results)
        {
            int index = _members.IndexOf(member);
            if (index < 0)
            {
                return;
            }

            _members.RemoveAt(index);
            Destroy(member.gameObject);

            foreach (Ingredient result in results)
            {
                Ingredient instance = Instantiate(result, transform);
                _members.Insert(index, instance);
                instance.Stack = this;
                index++;
            }

            Arrange();
        }

        public static bool CanMerge(Pickable held, Pickable target)
        {
            Group heldGroup = GroupOf(held);

            if (heldGroup.Stack != null)
            {
                return heldGroup.Stack != GroupOf(target).Stack;
            }

            return heldGroup.Root != GroupOf(target).Root;
        }

        public static void Merge(Pickable held, Pickable target, in InteractionContext context)
        {
            Group heldGroup = GroupOf(held);
            Group targetGroup = GroupOf(target);

            Group baseGroup = heldGroup.Role < targetGroup.Role ? heldGroup : targetGroup;
            Group topGroup = baseGroup.Root == heldGroup.Root ? targetGroup : heldGroup;

            IngredientStack stack = baseGroup.Stack ?? FormOn(baseGroup.Root);

            foreach (Pickable content in topGroup.Contents)
            {
                stack.Append(content);
            }

            topGroup.Stack?.Dissolve();

            if (baseGroup.Root != heldGroup.Root)
            {
                context.Release();
            }
        }

        private static Group GroupOf(Pickable pickable)
        {
            if (pickable is IngredientStack stack)
            {
                return new Group(stack._root, stack);
            }

            return new Group(pickable, pickable.Stack);
        }

        private static IngredientStack FormOn(Pickable root)
        {
            IngredientStack stack = root.gameObject.AddComponent<IngredientStack>();
            stack._root = root;
            root.Stack = stack;
            return stack;
        }

        private void Append(Pickable content)
        {
            _members.Add(content);
            content.Stack = this;
            content.transform.SetParent(transform);
            Arrange();
        }

        private void Arrange()
        {
            float height = _root.Data.StackHeight;

            foreach (Pickable member in _members)
            {
                member.transform.localPosition = new Vector3(0f, height, 0f);
                member.transform.localRotation = Quaternion.identity;
                height += member.Data.StackHeight;
            }
        }

        private void Dissolve()
        {
            Destroy(this);
        }

        private readonly struct Group
        {
            public Pickable Root { get; }
            public IngredientStack Stack { get; }
            public StackingRole Role => Root.Data.StackingRole;

            public IEnumerable<Pickable> Contents
            {
                get
                {
                    yield return Root;

                    if (Stack != null)
                    {
                        foreach (Pickable member in Stack._members)
                        {
                            yield return member;
                        }
                    }
                }
            }

            public Group(Pickable root, IngredientStack stack)
            {
                Root = root;
                Stack = stack;
            }
        }
    }
}
