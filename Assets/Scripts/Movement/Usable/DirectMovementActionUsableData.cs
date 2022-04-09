namespace SDefence.Movement.Usable
{
    using UnityEngine;

    public class DirectMovementActionUsableData : AbstractMovementActionUsableData, IMovementActionUsableData
    {
        public override void RunProcess(IMoveable moveable, IMovementUsableData data, float deltaTime, Vector2 target)
        {
            if (RunProcessTypeMovement(moveable, target, deltaTime) == TYPE_MOVEMENT_STATE.Run)
            {
                moveable.SetPosition(target);
            }
        }
    }
}
