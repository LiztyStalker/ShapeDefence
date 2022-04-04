namespace SDefence.Actor
{
    using SDefence.Attack;
    public interface IDamagable
    {
        public void SetDamage(IAttackUsableData data);
    }
}