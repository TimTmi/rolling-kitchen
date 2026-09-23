using System.Collections.Generic;
using Features.Dish;
using Features.Pickup;
using UnityEngine;
using UnityEngine.UIElements;

namespace Features.UI.OrderTickets
{
    public class OrderTicketsController : UIComponent
    {
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private VisualTreeAsset ticketTemplate;

        private ScrollView _list;
        private readonly List<TicketView> _tickets = new();

        protected override void BindElements(VisualElement root)
        {
            _list = root.Q<ScrollView>("OrderTickets");
        }

        protected override void OnDisabled()
        {
            _list?.Clear();
            _tickets.Clear();
        }

        private void Update()
        {
            if (_list == null || orderManager == null)
            {
                return;
            }

            int slotCount = orderManager.SlotCount;
            if (_tickets.Count != slotCount)
            {
                _list.Clear();
                _tickets.Clear();
                for (int i = 0; i < slotCount; i++)
                {
                    _tickets.Add(CreateTicket());
                }
            }

            for (int i = 0; i < _tickets.Count; i++)
            {
                Order order = orderManager.GetOrder(i);
                TicketView view = _tickets[i];
                if (!ReferenceEquals(order, view.Order))
                {
                    RebuildTicket(view, order);
                    view.Order = order;
                }

                UpdateTicket(view, i);
            }
        }

        private TicketView CreateTicket()
        {
            TemplateContainer element = ticketTemplate.Instantiate();
            _list.Add(element);
            return new TicketView { Root = element.Q("Ticket") };
        }

        private static void RebuildTicket(TicketView view, Order order)
        {
            view.Dishes.Clear();
            VisualElement dishes = view.Root.Q("Dishes");
            dishes.Clear();
            if (order == null)
            {
                return;
            }

            foreach (DishData dish in order.Dishes)
            {
                var dishRow = new VisualElement();
                dishRow.AddToClassList("ticket-dish");

                var name = new Label(dish.name);
                name.AddToClassList("ticket-dish-name");
                dishRow.Add(name);

                var icons = new VisualElement();
                icons.AddToClassList("ticket-ingredients");
                var ingredients = new List<VisualElement>();
                foreach (Pickup.Ingredient ingredient in dish.RequiredIngredients)
                {
                    var icon = new Image { sprite = ingredient.IngredientData.Icon, scaleMode = ScaleMode.ScaleToFit };
                    icon.AddToClassList("ticket-ingredient");
                    icons.Add(icon);
                    ingredients.Add(icon);
                }

                dishRow.Add(icons);
                dishes.Add(dishRow);
                var dishView = new DishView { Root = dishRow };
                dishView.Ingredients.AddRange(ingredients);
                view.Dishes.Add(dishView);
            }
        }

        private void UpdateTicket(TicketView view, int slotIndex)
        {
            view.Root.style.display = view.Order == null ? DisplayStyle.None : DisplayStyle.Flex;
            if (view.Order == null)
            {
                return;
            }

            var trayLabel = view.Root.Q<Label>("TrayLabel");
            trayLabel.text = $"Tray {slotIndex + 1}";

            for (int d = 0; d < view.Dishes.Count; d++)
            {
                DishView dishView = view.Dishes[d];
                dishView.Root.EnableInClassList("ticket-dish-complete", orderManager.IsDishComplete(slotIndex, d));
                for (int k = 0; k < dishView.Ingredients.Count; k++)
                {
                    dishView.Ingredients[k].EnableInClassList("ticket-ingredient-present",
                        orderManager.IsIngredientComplete(slotIndex, d, k));
                }
            }
        }

        private sealed class DishView
        {
            public VisualElement Root;
            public readonly List<VisualElement> Ingredients = new();
        }

        private sealed class TicketView
        {
            public VisualElement Root;
            public Order Order;
            public readonly List<DishView> Dishes = new();
        }
    }
}
