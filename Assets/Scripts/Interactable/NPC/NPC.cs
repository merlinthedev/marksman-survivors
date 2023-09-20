using EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public abstract class NPC : MonoBehaviour {
    private void OnMouseEnter() {
        EventBus<InteractableStartHoverEvent>.Raise(new InteractableStartHoverEvent());
    }

    private void OnMouseExit() {
        EventBus<InteractableStopHoverEvent>.Raise(new InteractableStopHoverEvent());
    }

    [SerializeField] public List<Dialogue.Dialogue> dialogue;

    public abstract void OnEndDialogue();
}
