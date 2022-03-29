namespace SDefence.Durable.Usable
{
    using System.Numerics;
    using Utility.Number;

    public abstract class AbstractDurableUsableData : IDurableUsableData
    {
        private BigDecimal _value;
        public BigDecimal Value { get => _value; protected set => _value = value; }


        public bool IsZero => Value.IsZero;
        public void SetZero() => Value = 0;


        public virtual void Add(int value) => Value += value;
        public virtual void Add(UniversalUsableData dData) => Value += dData.Value;
        public virtual void Subject(int value) => Value -= value;
        public virtual void Subject(UniversalUsableData dData) => Value -= dData.Value;     
        public virtual void Set(int value) => Value = value;
        public virtual void Set(IDurableUsableData value) => Value = ((AbstractDurableUsableData)value).Value;
        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade)
        {
            var sVal = long.Parse(startValue);
            var incVal = int.Parse(increaseValue);
            var incRate = float.Parse(increaseRate);

            Value = new BigDecimal(sVal);
            Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, upgrade);
        }



        public bool IsOverflowMaxValue(IDurableUsableData maxValue, UniversalUsableData value)
        {
            var maxVal = ((AbstractDurableUsableData)maxValue).Value;
            var val = value.Value;
            return (Value + val > maxVal);
        }

        public bool IsUnderflowZero(UniversalUsableData value)
        {
            var val = value.Value;
            return (Value - val < 0);
        }

        public int Compare(UniversalUsableData value)
        {
            if (Value - value.Value > 0) return -1;
            else if (Value - value.Value < 0) return 1;
            return 0;
        }

        public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);


        protected static T Create<T>() where T : IDurableUsableData
        {
            return System.Activator.CreateInstance<T>();
        }

        private static IDurableUsableData Create(System.Type type)
        {
            return (IDurableUsableData)System.Activator.CreateInstance(type);
        }


        public string ToString(string format) => Value.ToString(format);
        public override string ToString() => Value.ToString();


        public IDurableUsableData Clone()
        {
            var data = Create(GetType());
            data.Set(this);
            return data;
        }

    }
}