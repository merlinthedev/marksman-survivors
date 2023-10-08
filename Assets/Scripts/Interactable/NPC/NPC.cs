using Core;
using System.Collections.Generic;
using EventBus;
using UnityEngine;

namespace Interactable.NPC {
    public abstract class NPC : MonoBehaviour {
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