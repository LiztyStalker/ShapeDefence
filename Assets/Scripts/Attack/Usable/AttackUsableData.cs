

namespace SDefence.Attack.Usable
{
    using System.Numerics;
    using Utility.Number;

    public class AttackUsableData : IAttackUsableData
    {
        private BigDecimal _value;
        public BigDecimal Value { get => _value; protected set => _value = value; }
        public bool IsZero => Value.IsZero;
        public virtual void Set(int value) => Value = value;
        public virtual void Set(IAttackUsableData value) => Value = ((AttackUsableData)value).Value;

        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade)
        {
            var sVal = long.Parse(startValue);
            var incVal = int.Parse(increaseValue);
            var incRate = float.Parse(increaseRate);

            Value = new BigDecimal(sVal);
            Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, upgrade);
        }


        protected static T Create<T>() where T : IAttackUsableData
        {
            return System.Activator.CreateInstance<T>();
        }

        private static IAttackUsableData Create(System.Type type)
        {
            return (IAttackUsableData)System.Activator.CreateInstance(type);
        }

        public string ToString(string format) => Value.ToString(format);
        public override string ToString() => Value.ToString();


        public IAttackUsableData Clone()
        {
            var data = Create(GetType());
            data.Set(this);
            return data;
        }

        public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);
    }
}