using _Scripts.EventBus;

namespace _Scripts.Interactable.NPC {
    public class King : NPC, IInteractable {
        public void OnInteract() {
            // Debug.Log("Interacting with King");
            EventBus<StartDialogueEvent>.Raise(new StartDialogueEvent(this.dialogue, gameObject));
        }

        public override void OnEndDialogue() {
            // Debug.Log("Ending dialogue with King");
            EventBus<LoadSceneEvent>.Raise(new LoadSceneEvent("Run"));
        }
    }
}