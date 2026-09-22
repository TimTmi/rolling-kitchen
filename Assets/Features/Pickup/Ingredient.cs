using Features.Ingredient;

namespace Features.Pickup
{
    public class Ingredient : Pickable
    {
        public IngredientData IngredientData => (IngredientData)Data;
        public float CookingProgress { get; private set; }

        public void AddCookingProgress(float seconds)
        {
            CookingProgress += seconds;
        }

        public bool HasUnfinishedProcess(ProcessType type, out IngredientProcess process)
        {
            return IngredientData.TryGetProcess(type, out process) && CookingProgress < process.Duration;
        }
    }
}
