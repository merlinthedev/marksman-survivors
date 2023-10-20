namespace Champions.Abilities {
    public interface ICastable {
        float CastTime { get; set; }
        void Cast();
    }
}