using System.Collections.Generic;
using EventBus;
using UnityEngine;

namespace Champions.Abilities {
    public class CooldownManager : MonoBehaviour {
        private List<ICooldown> cooldowns = new();

        private void OnEnable() {
            EventBus<SubscribeICooldownEvent>.Subscribe(OnICooldownSubscribed);
            EventBus<UnsubscribeICooldownEvent>.Subscribe(OnICooldownUnsubscribed);
        }

        private void OnDisable() {
            EventBus<SubscribeICooldownEvent>.Unsubscribe(OnICooldownSubscribed);
            EventBus<UnsubscribeICooldownEvent>.Unsubscribe(OnICooldownUnsubscribed);
        }

        private void Update() { }

        private void TickCooldowns() {
            cooldowns.ForEach(cooldown => {
                if (cooldown.ShouldTick) {
                    cooldown.Tick(Time.deltaTime);
                }
            });
        }

        private void OnICooldownSubscribed(SubscribeICooldownEvent obj) {
            cooldowns.Add(obj.Cooldown);
        }

        private void OnICooldownUnsubscribed(UnsubscribeICooldownEvent obj) {
            cooldowns.Remove(obj.Cooldown);
        }
    }
}