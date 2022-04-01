namespace SDefence.Movement
{
    using UnityEngine;

    public interface IMoveable
    {
        public Vector2 NowPosition { get; }
        public void SetPosition(Vector2 pos);
    }
}