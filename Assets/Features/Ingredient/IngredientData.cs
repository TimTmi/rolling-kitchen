using System;
using System.Linq;
using Features.Pickables;
using UnityEngine;

namespace Features.Ingredient
{
    public enum ProcessType
    {
        Slice,
        Grill,
        DeepFry
    }

    [Serializable]
    public class IngredientProcess
    {
        [SerializeField] private ProcessType type;
        [SerializeField] private float duration;
        [SerializeField] private IngredientData result;

        public ProcessType Type => type;
        public float Duration => duration;
        public IngredientData Result => result;
    }
    
    [CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
    public class IngredientData : PickableData
    {
        public IngredientProcess[] processes;
        
        public bool HasProcess(ProcessType type) => processes.Any(x => x.Type == type);

        public bool TryGetProcess(ProcessType type, out IngredientProcess process)
        {
            process = processes.FirstOrDefault(x => x.Type == type);
            return process != null;
        }
    }
}
