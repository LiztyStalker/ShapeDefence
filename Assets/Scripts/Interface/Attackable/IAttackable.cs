
namespace SDefence.Attack
{
    using UnityEngine;
    public interface IAttackable
    {
        public Vector2 AttackPos { get; }

        public IAttackUsableData AttackUsableData { get; }
    }
}