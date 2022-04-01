namespace SDefence.Movement.Usable
{
    using UnityEngine;

    public class DirectMovementActionUsableData : AbstractMovementActionUsableData, IMovementActionUsableData
    {
        public override void RunProcess(IMoveable moveable, IMovementUsableData data, float deltaTime, Vector2 target)
        {
            base.RunProcess(moveable, data, deltaTime, target);
            moveable.SetPosition(target);
        }
    }
}
