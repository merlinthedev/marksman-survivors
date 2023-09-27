﻿namespace Champions.Abilities {
    public interface ICooldown {
        bool ShouldTick { get; set; }
        public void Tick(float deltaTime);
        public void Subscribe(ICooldown cooldown);
        public void Unsubscribe(ICooldown cooldown);
    }
}