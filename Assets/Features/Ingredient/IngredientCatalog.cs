using System;
using UnityEngine;

namespace Features.Ingredient
{
    [CreateAssetMenu(fileName = "IngredientCatalog", menuName = "Scriptable Objects/IngredientCatalog")]
    public class IngredientCatalog : ScriptableObject
    {
        [SerializeField] private IngredientData[] ingredients = Array.Empty<IngredientData>();

        public IngredientData[] Ingredients => ingredients;

        public void SetIngredients(IngredientData[] value)
        {
            ingredients = value;
        }
    }
}
