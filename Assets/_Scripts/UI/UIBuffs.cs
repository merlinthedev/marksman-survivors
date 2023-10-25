using System.Collections.Generic;
using EventBus;
using UnityEngine;
using UnityEngine.UI;
using static BuffsDebuffs.Stacks.Stack;

namespace UI {
    public class UIBuffs : MonoBehaviour {
        [Header("Icons")]
        [SerializeField] private Sprite m_DeftnessIcon;

        [SerializeField] private Sprite m_OverpowerIcon;
        [SerializeField] private Sprite m_FragileIcon;

        [System.Serializable]
        public class Slot {
            [SerializeField] public UIBuff UIBuff;
            [SerializeField] public StackType stackType;
        }

        [SerializeField] private List<Slot> m_Slots;

        private void OnEnable() {
            EventBus<ChangeStackUIEvent>.Subscribe(OnStacksChanged);
        }

        private void OnDisable() {
            EventBus<ChangeStackUIEvent>.Unsubscribe(OnStacksChanged);
        }

        private void OnStacksChanged(ChangeStackUIEvent e) {
            if (e.open) {
                for (int i = 0; i < m_Slots.Count; i++) {
                    if (m_Slots[i].stackType == e.type) {
                        m_Slots[i].UIBuff.SetStacks(e.stacks);
                        return;
                    }

                    if (m_Slots[i].stackType == StackType.defaultStack) {
                        m_Slots[i].stackType = e.type;

                        m_Slots[i].UIBuff.SetStacks(e.stacks);
                        m_Slots[i].UIBuff.GetComponent<Image>().sprite = ReturnSprite(e.type);
                        // Debug.Log("Added " + e.stacks + " " + e.type + " stacks");
                        return;
                    }
                }
            } else if (!e.open) {
                Debug.Log("Removing " + e.stacks + " " + e.type + " stacks");
                for (int i = 0; i < m_Slots.Count; i++) {
                    if (m_Slots[i].stackType == e.type) {
                        m_Slots[i].UIBuff.SetStacks(-e.stacks);
                        if (m_Slots[i].UIBuff.stacks <= 0) {
                            m_Slots[i].stackType = StackType.defaultStack;
                        }

                        return;
                    }
                }
            }
        }

        private Sprite ReturnSprite(StackType stacktype) {
            if (stacktype == StackType.DEFTNESS) {
                return m_DeftnessIcon;
            } else if (stacktype == StackType.OVERPOWER) {
                return m_OverpowerIcon;
            } else if (stacktype == StackType.FRAGILE) {
                return m_FragileIcon;
            } else {
                return null;
            }
        }
    }
}