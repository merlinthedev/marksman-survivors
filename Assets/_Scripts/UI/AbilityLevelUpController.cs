using System;
using UnityEngine;
using UnityEngine.UI;


namespace _Scripts.UI {
    public class AbilityLevelUpController : MonoBehaviour, ILevelPanelComponent {
        [SerializeField] private Button button;

        public void SetAction(Action action) {
            button.onClick.AddListener(() => { action(); });
        }
    }
}