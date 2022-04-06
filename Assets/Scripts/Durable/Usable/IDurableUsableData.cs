namespace SDefence.Durable
{
    using System.Numerics;

    public interface IDurableUsableData
    {
        public BigDecimal Value { get; }
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
        public float GetRate();
        public bool IsOverflowMaxValue(IDurableUsableData maxValue, UniversalUsableData value);
        public bool IsUnderflowZero(UniversalUsableData value);
        public int Compare(UniversalUsableData value);
        public System.Type GetType();
        public IDurableUsableData Clone();
        public UniversalUsableData CreateUniversalUsableData();

    }
}