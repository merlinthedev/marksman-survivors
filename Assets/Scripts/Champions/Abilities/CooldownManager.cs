using System.Collections.Generic;
using EventBus;
using UnityEngine;
using Logger = Util.Logger;

namespace Champions.Abilities {
    /// <summary>
    /// Class that takes care of ticking cooldowns for us. 
    /// </summary>
    public class CooldownManager : MonoBehaviour {
        private List<ICooldown> cooldowns = new();
        private bool shouldTickCooldowns = true;

        private void OnEnable() {
            EventBus<SubscribeICooldownEvent>.Subscribe(OnICooldownSubscribed);
            EventBus<UnsubscribeICooldownEvent>.Subscribe(OnICooldownUnsubscribed);

            EventBus<GamePausedEvent>.Subscribe(OnGamePaused);
            EventBus<GameResumedEvent>.Subscribe(OnGameResumed);
        }

        private void OnDisable() {
            EventBus<SubscribeICooldownEvent>.Unsubscribe(OnICooldownSubscribed);
            EventBus<UnsubscribeICooldownEvent>.Unsubscribe(OnICooldownUnsubscribed);

            EventBus<GamePausedEvent>.Unsubscribe(OnGamePaused);
            EventBus<GameResumedEvent>.Unsubscribe(OnGameResumed);
        }

        private void OnGameResumed(GameResumedEvent obj) {
            shouldTickCooldowns = true;
        }

        private void OnGamePaused(GamePausedEvent obj) {
            shouldTickCooldowns = false;
        }

        private void Update() {
            if (!shouldTickCooldowns) {
                // Logger.Log("Cooldowns are not ticking because the game is paused.", this);
                return;
            }

            TickCooldowns();
        }

        /// <summary>
        /// Ticks each cooldown in the list.
        /// </summary>
        private void TickCooldowns() {
            // Logger.Log("Ticking cooldowns.", this);
            cooldowns.ForEach(cooldown => {
                if (cooldown.ShouldTick) {
                    cooldown.Tick(Time.deltaTime);
                } else {
                    // Logger.Log("ShouldTick is false", this);
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