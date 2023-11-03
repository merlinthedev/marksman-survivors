namespace _Scripts.Champions.Abilities {
    public interface ICooldown {
        public bool ShouldTick { get; }
        public float Cooldown { get; set; }
        public float CurrentCooldown { get; set; }
        public void Tick(float deltaTime);
        public void Subscribe(ICooldown cooldown);
        public void Unsubscribe(ICooldown cooldown);
        public event System.Action OnCooldownCompleted;
    }
}