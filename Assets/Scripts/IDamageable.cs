
public interface IDamageable {
    bool IsBurning { get; set; }
    bool IsFragile { get; set; }
    float FragileStacks { get; set; }
    float LastFragileApplyTime { get; set; }

    void TakeFlatDamage(float damage);
    void TakeBurnDamage(float damage, float interval, float time);
}