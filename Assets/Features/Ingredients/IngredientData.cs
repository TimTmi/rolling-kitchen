using System;
using System.Linq;
using Features.Minigame;
using Features.Pickup;
using UnityEngine;

namespace Features.Ingredients
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
        [SerializeField] private Pickup.Ingredient[] result;
        [SerializeField] private CuttingMinigame minigame;

        public ProcessType Type => type;
        public float Duration => duration;
        public Pickup.Ingredient[] Result => result;
        public CuttingMinigame Minigame => minigame;
        public float ExpectedDuration => minigame != null ? minigame.ExpectedDuration : duration;
    }
    
    [CreateAssetMenu(fileName = "IngredientData", menuName = "Scriptable Objects/IngredientData")]
    public class IngredientData : PickableData
    {
        [SerializeField] private IngredientProcess[] processes;
        public IngredientProcess[] Processes => processes;
        
        public bool HasProcess(ProcessType type) => processes.Any(x => x.Type == type);

        public bool TryGetProcess(ProcessType type, out IngredientProcess process)
        {
            process = processes.FirstOrDefault(x => x.Type == type);
            return process != null;
        }
    }
}
