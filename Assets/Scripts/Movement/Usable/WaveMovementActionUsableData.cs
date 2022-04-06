namespace SDefence.Movement.Usable
{
    using UnityEngine;

    public class WaveMovementActionUsableData : AbstractMovementActionUsableData, IMovementActionUsableData
    {
        public override void RunProcess(IMoveable moveable, IMovementUsableData data, float deltaTime, Vector2 target)
        {
            base.RunProcess(moveable, data, deltaTime, target);

            var pos = Vector2.MoveTowards(moveable.NowPosition, target, data.MovementValue * deltaTime);
            //x�� y�� wave �ֱ�
            moveable.SetPosition(pos);
        }
    }
}
