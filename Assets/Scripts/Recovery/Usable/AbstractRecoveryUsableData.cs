namespace SDefence.Recovery.Usable
{
    using System.Numerics;
    using Utility.Number;

    public abstract class AbstractRecoveryUsableData : IRecoveryUsableData
    {
        private BigDecimal _value;
        public BigDecimal Value { get => _value; protected set => _value = value; }
        public bool IsZero => Value.IsZero;
        public virtual void Set(int value) => Value = value;
        public virtual void Set(IRecoveryUsableData value) => Value = ((AbstractRecoveryUsableData)value).Value;

        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade)
        {
            var sVal = long.Parse(startValue);
            var incVal = int.Parse(increaseValue);
            var incRate = float.Parse(increaseRate);

            Value = new BigDecimal(sVal);
            Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, upgrade);
        }


        protected static T Create<T>() where T : IRecoveryUsableData
        {
            return System.Activator.CreateInstance<T>();
        }

        private static IRecoveryUsableData Create(System.Type type)
        {
            return (IRecoveryUsableData)System.Activator.CreateInstance(type);
        }

        public string ToString(string format) => Value.ToString(format);
        public override string ToString() => Value.ToString();

        public abstract string DurableKey();

        public IRecoveryUsableData Clone()
        {
            var data = Create(GetType());
            data.Set(this);
            return data;
        }

        public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);
    }
}