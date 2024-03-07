using _Scripts.BuffsDebuffs.Stacks;
using _Scripts.Core;
using _Scripts.EventBus;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Interactable.NPC {
    public abstract class NPC : MonoBehaviour, IEntity {
        private void OnMouseEnter() {
            EventBus<InteractableStartHoverEvent>.Raise(new InteractableStartHoverEvent());
        }

        private void OnMouseExit() {
            EventBus<InteractableStopHoverEvent>.Raise(new InteractableStopHoverEvent());
        }

        [SerializeField] public List<Dialogue> dialogue;

        public abstract void OnEndDialogue();
    }
}