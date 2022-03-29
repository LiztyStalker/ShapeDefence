namespace SDefence.Attack
{
    public interface IAttackUsableData
    {
        public bool IsZero { get; }
        public void Set(IAttackUsableData value);
        public void SetData(string startValue, string increaseValue, string increaseRate, string startDelayValue, string decreaseDelayValue, string decreaseDelayRate, int upgrade);
        public string ToString(string format);
        public UniversalUsableData CreateUniversalUsableData();
        public IAttackUsableData Clone();
    }
}