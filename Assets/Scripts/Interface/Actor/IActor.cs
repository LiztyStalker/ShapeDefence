
namespace SDefence.Actor
{
    using Durable;

    public interface IActor
    {
        public void Activate();
        public void Inactivate();
        public string GetDurableValue<T>() where T : IDurableUsableData;
        public float GetDurableRate<T>() where T : IDurableUsableData;
    }
}