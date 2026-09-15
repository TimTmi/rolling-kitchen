using System;
using UnityEngine;

namespace Features.Pickables
{
    [Flags]
    public enum IngredientCapability
    {
        None = 0,
        Sliceable = 1 << 0,
        Grillable = 1 << 1,
        Deepfryable = 1 << 2
    }
    
    [CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
    public class IngredientData : PickableData
    {
        public IngredientCapability capabilities;
    }
}
