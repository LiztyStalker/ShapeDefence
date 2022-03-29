namespace SDefence.Durable
{
    using Recovery;
    using Attack;

    public interface IDurableUsableData
    {
        public bool IsZero { get; }
        public void Add(int value);
        public void Add(UniversalUsableData value);
        public void Subject(int value);
        public void Subject(UniversalUsableData value);
        public void Set(int value);
        public void Set(IDurableUsableData value);
        public void SetZero();
        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade);
        public string ToString(string format);
        public bool IsOverflowMaxValue(IDurableUsableData maxValue, UniversalUsableData value);
        public bool IsUnderflowZero(UniversalUsableData value);
        public int Compare(UniversalUsableData value);
        public System.Type GetType();
        public IDurableUsableData Clone();
        public UniversalUsableData CreateUniversalUsableData();

    }
}