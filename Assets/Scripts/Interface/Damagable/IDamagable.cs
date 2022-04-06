namespace SDefence.Actor
{
    using SDefence.Attack;
    public interface IDamagable
    {
        public bool IsDamagable { get; }
        public void SetDamage(IAttackUsableData data);
    }
}