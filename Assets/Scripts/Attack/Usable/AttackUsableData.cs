namespace SDefence.Attack.Usable
{
    using SDefence.Durable;
    using System.Numerics;
    using Utility.Number;

    public class AttackUsableData : IAttackUsableData
    {
        
        private BigDecimal _value;
        public BigDecimal Value { get => _value; private set => _value = value; }
                

        private float _delay;
        public float Delay { get => _delay; private set => _delay = value; }


        private float _range;

        public float Range { get => _range; private set => _range = value; }


        public bool IsZero => Value.IsZero;


        public virtual void Set(IAttackUsableData value)
        {
            Value = ((AttackUsableData)value).Value;
            Delay = ((AttackUsableData)value).Delay;
            Range = ((AttackUsableData)value).Range;
        }

        public void SetAttack(string startValue, string increaseValue, string increaseRate, int upgrade)
        {
            var incVal = int.Parse(increaseValue);
            var incRate = float.Parse(increaseRate);

            Value = new BigDecimal(startValue);
            Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, upgrade);           
        }

        public void SetDelay(string startValue, string decreaseValue, string decreaseRate, int upgrade)
        {
            var startVal = float.Parse(startValue);
            var decVal = float.Parse(decreaseValue);
            var decRate = float.Parse(decreaseRate);

            Delay = startVal;
            Delay = NumberDataUtility.GetIsolationInterest(Delay, decVal, decRate, upgrade);

        }

        public void SetRange(string startValue, string increaseValue, string increaseRate, int upgrade)
        {
            var startVal = float.Parse(startValue);
            var incVal = float.Parse(increaseValue);
            var incRate = float.Parse(increaseRate);

            Range = startVal;
            Range = NumberDataUtility.GetIsolationInterest(Delay, incVal, incRate, upgrade);

        }

        public void SetData(UniversalUsableData data)
        {
            _value = data.Value;
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
        
        
        public UniversalUsableData CreateUniversalUsableData() => new UniversalUsableData(Value);

        public IAttackUsableData Clone()
        {
            var data = Create(GetType());
            data.Set(this);
            return data;
        }

    }
}