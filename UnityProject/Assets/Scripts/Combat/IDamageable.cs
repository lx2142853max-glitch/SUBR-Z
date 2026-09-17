namespace SUBR.Combat
{
    public interface IDamageable
    {
        int ActorId { get; }
        bool IsDead { get; }
        float ApplyDamage(float amount, int attackerId, bool headshot);
    }
}
