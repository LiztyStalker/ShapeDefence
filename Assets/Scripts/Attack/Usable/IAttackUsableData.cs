namespace SDefence.Attack
{
    public interface IAttackUsableData
    {
        public bool IsZero { get; }
        public void Set(int value);
        public void Set(IAttackUsableData value);
        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade);
        public string ToString(string format);
        public IAttackUsableData Clone();

        public UniversalUsableData CreateUniversalUsableData();
    }
}