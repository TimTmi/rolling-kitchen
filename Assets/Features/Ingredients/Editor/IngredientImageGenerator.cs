using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Features.Ingredient.Editor
{
    public static class IngredientImageGenerator
    {
        const string PrefabFolder = "Assets/ThirdParty/Unity/Toony Kitchen Ingredients Free/Prefabs/Ingredients";
        const string OutputFolder = "Assets/Features/Ingredients/Art";

        [MenuItem("Tools/Generate Ingredient Images")]
        public static void GenerateAll()
        {
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No prefabs found in {PrefabFolder}");
                return;
            }

            var prefabs = new List<GameObject>(guids.Length);
            foreach (var guid in guids)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
                if (prefab != null)
                    prefabs.Add(prefab);
            }

            Features.Editor.PrefabImageRenderer.RenderPrefabs(prefabs, OutputFolder, "Ingredient Images");
        }
    }
}
