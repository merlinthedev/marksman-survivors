namespace _Scripts.Entities {
    public interface IAttachable {
        void OnAttach(IDamageable damageable, IDamager source);
        void OnUse();
    }
}