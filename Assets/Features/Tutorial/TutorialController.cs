using System;
using System.Collections.Generic;
using System.Linq;
using Features.Dish;
using Features.Ingredients;
using Features.Interaction;
using Features.Pickup;
using Features.Player;
using Features.Service;
using Features.UI.HUD;
using UnityEngine;

namespace Features.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private ServiceConfig serviceConfig;
        [SerializeField] private HandController handController;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private Griddle griddle;
        [SerializeField] private Deepfryer deepfryer;
        [SerializeField] private CuttingPlate cuttingPlate;
        [SerializeField] private Fridge fridge;
        [SerializeField] private HUDController hud;

        [SerializeField] private PickableData box;
        [SerializeField] private PickableData bun;
        [SerializeField] private PickableData bottomBun;
        [SerializeField] private PickableData topBun;
        [SerializeField] private PickableData rawPatty;
        [SerializeField] private PickableData cookedPatty;
        [SerializeField] private PickableData cheese;
        [SerializeField] private PickableData meltedCheese;
        [SerializeField] private PickableData tomato;
        [SerializeField] private PickableData slicedTomato;
        [SerializeField] private PickableData rawFries;
        [SerializeField] private PickableData cookedFries;

        private TutorialStep[] _steps;
        private int _index;
        private float _timer;
        private int _fridgeOpens;
        private int _fridgeOpensAtStepStart;
        private bool _served;

        private struct TutorialStep
        {
            public string Hint;
            public float AutoAdvanceSeconds;
            public Action OnStart;
            public Func<bool> IsComplete;
        }

        private void Awake()
        {
            if (serviceConfig.GameMode != GameMode.Tutorial)
            {
                enabled = false;
            }
        }

        private void OnEnable()
        {
            fridge.Opened += OnFridgeOpened;
            orderManager.Served += OnServed;
        }

        private void OnDisable()
        {
            fridge.Opened -= OnFridgeOpened;
            orderManager.Served -= OnServed;
        }

        private void Start()
        {
            _steps = CreateSteps();
            StartStep(_steps[0]);
        }

        private void Update()
        {
            if (_index >= _steps.Length)
            {
                return;
            }

            TutorialStep step = _steps[_index];
            if (step.IsComplete != null)
            {
                if (step.IsComplete())
                {
                    Advance();
                }

                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= step.AutoAdvanceSeconds)
            {
                Advance();
            }
        }

        private void Advance()
        {
            _index++;
            if (_index >= _steps.Length)
            {
                hud.SetTutorialHint(string.Empty);
                return;
            }

            StartStep(_steps[_index]);
        }

        private void StartStep(TutorialStep step)
        {
            _timer = 0f;
            step.OnStart?.Invoke();
            hud.SetTutorialHint(step.Hint);
        }

        private TutorialStep[] CreateSteps()
        {
            return new TutorialStep[]
            {
                new TutorialStep { Hint = "Wait for the customer to order.", IsComplete = () => OrderTray() != null },
                new TutorialStep { Hint = "The tickets on the left list what the customer wants.", AutoAdvanceSeconds = 5f },
                new TutorialStep { Hint = "The counter top-left shows how many orders are left.", AutoAdvanceSeconds = 5f },
                new TutorialStep { Hint = "The bar at the top is your reputation. It drops when an order times out.", AutoAdvanceSeconds = 5f },
                new TutorialStep { Hint = "Take a box and place it on the tray in front of the customer.", IsComplete = () => TrayContains(box) },
                new TutorialStep { Hint = "Open the fridge.", OnStart = MarkFridgeOpen, IsComplete = FridgeOpenedSinceStepStart },
                new TutorialStep { Hint = "Take a bun from the fridge.", IsComplete = () => HeldIs(bun) },
                new TutorialStep { Hint = "Place the bun on the cutting board and slice it.", IsComplete = () => PlateContains(bottomBun) },
                new TutorialStep { Hint = "Place the bottom bun on the tray.", IsComplete = () => TrayContains(bottomBun) },
                new TutorialStep { Hint = "Take a patty and place it on the griddle.", IsComplete = () => GriddleContains(rawPatty) },
                new TutorialStep { Hint = "Get cheese and place it on top of the patty.", IsComplete = () => GriddleContains(cheese) },
                new TutorialStep { Hint = "Wait for the patty and cheese to finish cooking.", IsComplete = () => GriddleContains(cookedPatty) && GriddleContains(meltedCheese) },
                new TutorialStep { Hint = "Take them and place them on the bun on the tray.", IsComplete = () => TrayContains(cookedPatty) && TrayContains(meltedCheese) },
                new TutorialStep { Hint = "Take a tomato from the fridge and slice it on the cutting board.", IsComplete = () => PlateContains(slicedTomato) },
                new TutorialStep { Hint = "Place the tomato on the stack.", IsComplete = () => TrayContains(slicedTomato) },
                new TutorialStep { Hint = "Place the top bun on the stack.", IsComplete = () => TrayContains(topBun) },
                new TutorialStep { Hint = "Take a potato and slice it on the cutting board.", IsComplete = () => PlateContains(rawFries) },
                new TutorialStep { Hint = "Take the raw fries to the deep fryer and fry them.", IsComplete = () => FryerContains(rawFries) },
                new TutorialStep { Hint = "Wait for the fries to finish frying.", IsComplete = () => FryerContains(cookedFries) },
                new TutorialStep { Hint = "Take a box and use it to take the fries out of the deep fryer.", IsComplete = () => HeldContains(box) && HeldContains(cookedFries) },
                new TutorialStep { Hint = "Place the fries box on the tray.", IsComplete = () => TrayContains(box) && TrayContains(cookedFries) },
                new TutorialStep { Hint = "Serve the customer!", IsComplete = () => _served }
            };
        }

        private void MarkFridgeOpen()
        {
            _fridgeOpensAtStepStart = _fridgeOpens;
        }

        private bool FridgeOpenedSinceStepStart()
        {
            return _fridgeOpens > _fridgeOpensAtStepStart;
        }

        private void OnFridgeOpened(IReadOnlyList<Ingredient> pickables)
        {
            _fridgeOpens++;
        }

        private void OnServed(int slotIndex, Order order)
        {
            _served = true;
        }

        private Tray OrderTray()
        {
            for (int i = 0; i < orderManager.SlotCount; i++)
            {
                if (orderManager.GetOrder(i) != null)
                {
                    return orderManager.GetSlot(i).Tray;
                }
            }

            return null;
        }

        private bool HeldIs(PickableData data)
        {
            return handController.HeldPickable != null && handController.HeldPickable.Data == data;
        }

        private bool HeldContains(PickableData data)
        {
            return ContainsData(Expand(handController.HeldPickable), data);
        }

        private bool TrayContains(PickableData data)
        {
            Tray tray = OrderTray();
            return tray != null && ContainsData(tray.Contents.SelectMany(Expand), data);
        }

        private bool GriddleContains(PickableData data)
        {
            return ContainsData(griddle.Contents.SelectMany(Expand), data);
        }

        private bool PlateContains(PickableData data)
        {
            return ContainsData(cuttingPlate.Contents.SelectMany(Expand), data);
        }

        private bool FryerContains(PickableData data)
        {
            return ContainsData(Expand(deepfryer.Frying), data);
        }

        private static bool ContainsData(IEnumerable<Pickable> pickables, PickableData data)
        {
            return pickables.Any(pickable => pickable != null && pickable.Data == data);
        }

        private static IEnumerable<Pickable> Expand(Pickable pickable)
        {
            return pickable switch
            {
                null => Enumerable.Empty<Pickable>(),
                IngredientStack stack => stack.Contents,
                _ => pickable.Stack != null ? pickable.Stack.Contents : new[] { pickable }
            };
        }
    }
}
