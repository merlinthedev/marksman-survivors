using EventBus;
using Interactable.NPC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Core {
    [System.Serializable]
    public class Dialogue {
        public string title;
        public string body;
    }

    public class DialogueManager : MonoBehaviour {
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text body;

        [SerializeField] private List<Dialogue> dialogue;
        private int pos = 0;

        [SerializeField] private GameObject currentNPC;

        private void OnEnable() {
            EventBus<StartDialogueEvent>.Subscribe(OnStartDialogue);
        }

        private void OnDisable() {
            EventBus<StartDialogueEvent>.Unsubscribe(OnStartDialogue);
        }

        private void OnStartDialogue(StartDialogueEvent e) {
            if(e.npc != null) {
                currentNPC = e.npc;
            }
        

            dialogue = e.dialogue;
            title.text = dialogue[pos].title;
            body.text = dialogue[pos].body;

            dialogueBox.SetActive(true);
        }

        public void OnContinueDialogue() {
            if(pos < dialogue.Count - 1) {
                pos++;
                title.text = dialogue[pos].title;
                body.text = dialogue[pos].body;
            }
            else {
                OnEndDialogue();
            }
        }

        private void OnEndDialogue() {
            dialogueBox.SetActive(false);
            pos = 0;

            if(currentNPC != null) {
                currentNPC.GetComponent<NPC>().OnEndDialogue();
            }
        }


    }
}