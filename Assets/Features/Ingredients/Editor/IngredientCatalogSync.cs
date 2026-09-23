using System.Linq;
using Features.Ingredients;
using UnityEditor;
using UnityEngine;

namespace Features.Ingredient.Editor
{
    public class IngredientCatalogSync : AssetPostprocessor
    {
        const string CatalogPath = "Assets/Features/Ingredient/Resources/IngredientData.asset";

        [InitializeOnLoadMethod]
        static void SyncOnLoad()
        {
            Sync();
        }

        static void OnPostprocessAllAssets(string[] addedAssets, string[] removedAssets, string[] movedAssets,
            string[] movedFromAssetPaths, string[] movedToAssetPaths, bool didMove)
        {
            Sync();
        }

        static void Sync()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<IngredientCatalog>(CatalogPath);
            if (catalog == null)
            {
                return;
            }

            IngredientData[] all = AssetDatabase.FindAssets("t:IngredientData")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<IngredientData>)
                .Where(data => data != null)
                .OrderBy(data => data.Id)
                .ToArray();

            if (catalog.Ingredients.SequenceEqual(all))
            {
                return;
            }

            catalog.SetIngredients(all);
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
        }
    }
}
