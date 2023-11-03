using _Scripts.Champions.Abilities;
using _Scripts.Champions.Kitegirl.Abilities.Ultimate;
using _Scripts.EventBus;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace _Scripts.Champions.Effects.impl {
    public class OverdriveEffect : Effect, ICooldown {
        [SerializeField] private float percentage = 0.1f;
        [SerializeField] private Statistic statistic;
        [SerializeField] private float duration = 10f;
        private float interval = 1f;
        private float currentCooldown;
        private float timeLeft;
        private float durationLeft;
        [SerializeField] private float factor;
        private float overflow = 0;

        public bool ShouldTick {
            get { return timeLeft > 0; }
        }

        public float Cooldown {
            get => currentCooldown;
            set => currentCooldown = value;
        }

        public float CurrentCooldown {
            get => timeLeft;
            set => timeLeft = value;
        }

        private Overdrive overdrive;

        public void Link(Overdrive overdrive) {
            this.overdrive = overdrive;
        }

        public override void OnApply() {
            Subscribe(this);
            currentCooldown = interval;
            timeLeft = currentCooldown;
            durationLeft = duration;
            OnCooldownCompleted += OnInterval;
        }

        public override void OnExpire() {
            Unsubscribe(this);
            OnCooldownCompleted -= OnInterval;
        }

        private void OnInterval() {
            // Debug.Log("Interval completed.");
            var result = this.champion.GetChampionStatistics()
                .TryAddMana(this.champion.GetChampionStatistics().MaxMana * percentage);

            // Debug.Log(result);
            if (!result.Key()) {
                var stacks = result.Value() / factor;
                overflow += stacks;
                if (overflow >= 1) {
                    // Debug.Log("Adding " + s + " stacks.");
                    overdrive.OnInterval((int)overflow);
                    overflow--;
                }
            }
        }

        public void Tick(float deltaTime) {
            timeLeft -= deltaTime;
            durationLeft -= deltaTime;

            if (timeLeft <= 0) {
                OnCooldownCompleted?.Invoke();

                timeLeft = currentCooldown;
            }

            if (durationLeft <= 0) {
                OnExpire();
            }
        }

        public void Subscribe(ICooldown cooldown) {
            EventBus<SubscribeICooldownEvent>.Raise(new SubscribeICooldownEvent(cooldown));
        }

        public void Unsubscribe(ICooldown cooldown) {
            EventBus<UnsubscribeICooldownEvent>.Raise(new UnsubscribeICooldownEvent(cooldown));
        }

        public event Action OnCooldownCompleted;
    }
}