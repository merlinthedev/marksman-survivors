using System;
using EventBus;

namespace Champions.Abilities {
    public class Dash : ICooldown {
        private readonly float cooldown;
        private float timeLeft;
        public bool ShouldTick => timeLeft > 0;

        public Dash(float cooldown) {
            this.cooldown = cooldown;

            Subscribe(this);
        }

        ~Dash() {
            Unsubscribe(this);

            OnCooldownCompleted = null;
        }

        public void Tick(float deltaTime) {
            timeLeft -= deltaTime;

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

        public event Action OnCooldownCompleted;
    }
}