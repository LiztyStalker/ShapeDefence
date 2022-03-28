namespace SDefence.Durable
{
    using Utility.Number;

    public interface IDurableUsableData
    {
        public bool IsZero { get; }
        public void Add(int value);
        public void Add(IDurableUsableData value);
        public void Subject(int value);
        public void Subject(IDurableUsableData value);
        public void Set(int value);
        public void Set(IDurableUsableData value);
        public void SetZero();
        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade);
        public string ToString(string format);
        public bool IsOverflowMaxValue(IDurableUsableData maxValue, IDurableUsableData value);
        public bool IsUnderflowZero(IDurableUsableData value);
        public int Compare(IDurableUsableData value);
        public System.Type GetType();
        public IDurableUsableData Clone();

    }
}