using System;
using System.Collections.Generic;
using Features.Interaction;
using Features.Pickables;
using Features.Player;
using Features.UI.PickableSelection;
using UnityEngine;

namespace Features.Service
{
    public class ServiceController : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private FridgeDoorInteractable fridgeDoorInteractable;
        [SerializeField] private PickableSelectionController  pickableSelectionController;
        
        void Start()
        {
            Core.CursorController.Lock();
        }

        private void OnEnable()
        {
            fridgeDoorInteractable.Opened += OnFridgeOpened;
        }

        private void OnDisable()
        {
            fridgeDoorInteractable.Opened -= OnFridgeOpened;
        }

        private void OnFridgeOpened(IReadOnlyList<PickableData> pickables)
        {
            playerController.SetInputActive(false);
            Core.CursorController.Unlock();
            ShowPickableSelection(pickables);
        }

        private void ShowPickableSelection(IReadOnlyList<PickableData> pickables) => pickableSelectionController.Show(pickables);
    }
}
