namespace SDefence.Recovery
{
    public interface IRecoveryUsableData
    {
        public bool IsZero { get; }
        public void Set(int value);
        public void Set(IRecoveryUsableData value);
        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade);
        public string ToString(string format);
        public string DurableKey();
        public IRecoveryUsableData Clone();

        public UniversalUsableData CreateUniversalUsableData();
    }
}