using _Scripts.EventBus;
using System;
using UnityEngine;

namespace _Scripts.Champions.Abilities {
    public class Dodge : ICooldown {
        private float cooldown;
        private float timeLeft;
        public bool ShouldTick => timeLeft > 0;

        public float Cooldown {
            get => cooldown;
            set => cooldown = value;
        }

        public float CurrentCooldown {
            get => timeLeft;
            set => timeLeft = value;
        }


        public Dodge(float cooldown) {
            this.cooldown = cooldown;

            Subscribe(this);
        }

        ~Dodge() {
            Unsubscribe(this);

            OnCooldownCompleted = null;
        }

        public void Tick(float deltaTime) {
            timeLeft -= deltaTime;
            // Debug.LogWarning("Ticking the cooldown: new cooldown: " + timeLeft);

            if (timeLeft <= 0) {
                OnCooldownCompleted?.Invoke();
            }
        }

        public void StartCooldown() {
            timeLeft = cooldown;
        }

        public void Subscribe(ICooldown cooldown) {
            EventBus<SubscribeICooldownEvent>.Raise(new SubscribeICooldownEvent(cooldown));
        }

        public void Unsubscribe(ICooldown cooldown) {
            EventBus<UnsubscribeICooldownEvent>.Raise(new UnsubscribeICooldownEvent(cooldown));
        }

        public bool IsOnCooldown() {
            return timeLeft > 0;
        }

        public void ResetCooldown() {
            Debug.LogWarning("RESETTING THE COOLDOWN");
            timeLeft = 0f;
        }

        public event Action OnCooldownCompleted;
    }
}