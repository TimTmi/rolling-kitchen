using System;
using System.Linq;
using UnityEngine;

namespace Features.Pickables
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
        public ProcessType type;
        public IngredientData[] results;
    }
    
    [CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
    public class IngredientData : PickableData
    {
        public IngredientProcess[] processes;
        
        public bool HasProcess(ProcessType type) => processes.Any(x => x.type == type);

        public bool TryGetProcess(ProcessType type, out IngredientProcess process)
        {
            process = processes.FirstOrDefault(x => x.type == type);
            return process != null;
        }
    }
}
