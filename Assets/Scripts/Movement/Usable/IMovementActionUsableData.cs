namespace SDefence.Movement
{
    using UnityEngine;
    public interface IMovementActionUsableData
    {
        public void RunProcess(IMoveable moveable, IMovementUsableData data, float deltaTime, Vector2 target);
        public void SetCollision();
        //public void SetData(float movementWeight, TYPE_MOVEMENT_ARRIVE typeArrived, TYPE_MOVEMENT_COLLISION typeCollision, TYPE_MOVEMENT_TARGET typeTarget, float _accuracy);
        public void SetData(float accuracy);
        public void SetOnStartActionListener(System.Action act);
        public void SetOnEndedActionListener(System.Action act);
    }
}