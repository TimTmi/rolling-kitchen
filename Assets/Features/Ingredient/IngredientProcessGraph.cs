using System;
using System.Collections.Generic;
using Features.Pickup;
using UnityEngine;

namespace Features.Ingredient
{
    public static class IngredientProcessGraph
    {
        public readonly struct Producer
        {
            public readonly IngredientData Source;
            public readonly IngredientProcess Process;

            public Producer(IngredientData source, IngredientProcess process)
            {
                Source = source;
                Process = process;
            }
        }

        const string CatalogResourcePath = "IngredientData";

        static readonly Dictionary<IngredientData, List<Producer>> ProducersByResult = new();
        static bool _built;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void BuildAtStartup()
        {
            _built = false;
            EnsureBuilt();
        }

        public static IReadOnlyList<Producer> GetProducers(IngredientData ingredient)
        {
            EnsureBuilt();
            return ProducersByResult.TryGetValue(ingredient, out List<Producer> producers)
                ? producers
                : Array.Empty<Producer>();
        }

        public static bool TryGetProcessPath(IngredientData ingredient, out IReadOnlyList<IngredientProcess> path)
        {
            EnsureBuilt();
            List<IngredientProcess> steps = new();
            if (!Backtrack(ingredient, new HashSet<IngredientData>(), steps))
            {
                path = Array.Empty<IngredientProcess>();
                return false;
            }

            steps.Reverse();
            path = steps;
            return true;
        }

        public static float ExpectedDuration(IngredientData ingredient)
        {
            return ExpectedDuration(new[] { ingredient });
        }

        public static float ExpectedDuration(IReadOnlyList<IngredientData> ingredients)
        {
            EnsureBuilt();

            Dictionary<IngredientData, int> demand = new();
            foreach (IngredientData ingredient in ingredients)
            {
                demand[ingredient] = demand.GetValueOrDefault(ingredient) + 1;
            }

            HashSet<IngredientData> propagated = new();
            HashSet<IngredientProcess> counted = new();
            float total = 0f;
            foreach (IngredientData ingredient in ingredients)
            {
                IngredientData current = ingredient;
                while (propagated.Add(current)
                       && ProducersByResult.TryGetValue(current, out List<Producer> producers))
                {
                    IngredientProcess process = producers[0].Process;
                    if (!counted.Add(process))
                    {
                        break;
                    }

                    int executions = 0;
                    foreach (Pickup.Ingredient result in process.Result)
                    {
                        if (result != null)
                        {
                            executions = Mathf.Max(executions, demand.GetValueOrDefault(result.IngredientData));
                        }
                    }

                    IngredientData source = producers[0].Source;
                    demand[source] = demand.GetValueOrDefault(source) + executions;
                    total += executions * process.ExpectedDuration;
                    current = source;
                }
            }

            return total;
        }

        static void EnsureBuilt()
        {
            if (_built)
            {
                return;
            }

            ProducersByResult.Clear();

            IngredientCatalog catalog = Resources.Load<IngredientCatalog>(CatalogResourcePath);
            if (catalog == null)
            {
                Debug.LogWarning($"Missing {nameof(IngredientCatalog)} at Resources/{CatalogResourcePath}");
                _built = true;
                return;
            }

            foreach (IngredientData data in catalog.Ingredients)
            {
                if (data == null)
                {
                    continue;
                }

                foreach (IngredientProcess process in data.Processes)
                {
                    foreach (Pickup.Ingredient result in process.Result)
                    {
                        if (result == null || result.IngredientData == null)
                        {
                            continue;
                        }

                        if (!ProducersByResult.TryGetValue(result.IngredientData, out List<Producer> producers))
                        {
                            producers = new List<Producer>();
                            ProducersByResult.Add(result.IngredientData, producers);
                        }

                        producers.Add(new Producer(data, process));
                    }
                }
            }

            _built = true;
        }

        static bool Backtrack(IngredientData ingredient, HashSet<IngredientData> visited, List<IngredientProcess> path)
        {
            if (!ProducersByResult.TryGetValue(ingredient, out List<Producer> producers))
            {
                return true;
            }

            if (!visited.Add(ingredient))
            {
                return false;
            }

            foreach (Producer producer in producers)
            {
                path.Add(producer.Process);
                if (Backtrack(producer.Source, visited, path))
                {
                    return true;
                }

                path.RemoveAt(path.Count - 1);
            }

            return false;
        }
    }
}
