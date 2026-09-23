using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Features.Editor
{
    /// Batch-renders user-picked prefabs to transparent PNGs in a chosen folder.
    public class PrefabImageGeneratorWindow : EditorWindow
    {
        [SerializeField] List<GameObject> prefabs = new();
        [SerializeField] string outputFolder = "Assets/Art";

        Vector2 scrollPosition;

        [MenuItem("Tools/Generate Prefab Images…")]
        static void Open()
        {
            GetWindow<PrefabImageGeneratorWindow>("Prefab Images");
        }

        void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
                if (GUILayout.Button("Browse", GUILayout.Width(60f)))
                {
                    var chosen = EditorUtility.OpenFolderPanel("Output Folder", outputFolder, "");
                    if (!string.IsNullOrEmpty(chosen))
                    {
                        var relative = ToProjectRelativePath(chosen);
                        if (relative != null)
                            outputFolder = relative;
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(240f));

            var removeIndex = -1;
            for (var i = 0; i < prefabs.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    prefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);
                    if (GUILayout.Button("−", GUILayout.Width(24f)))
                        removeIndex = i;
                }
            }

            EditorGUILayout.EndScrollView();

            // Never mutate the list mid-layout; ImGui requires it stable between
            // the Layout and Repaint passes.
            if (removeIndex >= 0)
                prefabs.RemoveAt(removeIndex);

            if (GUILayout.Button("Add Prefab"))
                prefabs.Add(null);

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(prefabs.Count == 0))
            {
                if (GUILayout.Button("Generate"))
                    Generate();
            }
        }

        void Generate()
        {
            prefabs.RemoveAll(p => p == null);
            if (prefabs.Count == 0)
                return;

            PrefabImageRenderer.RenderPrefabs(prefabs, outputFolder, "Prefab Images");
        }

        static string ToProjectRelativePath(string absolutePath)
        {
            if (!absolutePath.Replace('\\', '/').StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("Prefab Image Generator", "Please pick a folder inside the project's Assets folder.", "OK");
                return null;
            }

            var relative = absolutePath.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
            return relative.Length == 0 ? "Assets" : $"Assets/{relative}";
        }
    }
}
