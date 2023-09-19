using EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    private void OnMouseEnter() {
        EventBus<InteractableStartHoverEvent>.Raise(new InteractableStartHoverEvent());
    }

    private void OnMouseExit() {
        EventBus<InteractableStopHoverEvent>.Raise(new InteractableStopHoverEvent());
    }

    public void OnInteract() {
        Debug.Log("Interacting with NPC");
    }
}
