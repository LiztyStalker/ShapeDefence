
namespace SDefence.Actor
{
    using Durable;
    using UnityEngine;

    public interface IActor
    {
        public Vector2 NowPosition { get; }
        public void Activate();
        public void Inactivate();
        public string GetDurableValue<T>() where T : IDurableUsableData;
        public float GetDurableRate<T>() where T : IDurableUsableData;
    }
}