namespace SDefence.Attack.Usable
{
    using System.Numerics;
    using Utility.Number;

    public class AttackUsableData : IAttackUsableData
    {
        
        private BigDecimal _value;
        public BigDecimal Value { get => _value; private set => _value = value; }
                

        private float _delay;
        public float Delay { get => _delay; private set => _delay = value; }
        

        public bool IsZero => Value.IsZero;


        public virtual void Set(IAttackUsableData value)
        {
            Value = ((AttackUsableData)value).Value;
            Delay = ((AttackUsableData)value).Delay;
        }

        public void SetData(string startValue, string increaseValue, string increaseRate, string startDelayValue, string decreaseDelayValue, string decreaseDelayRate, int upgrade)
        {
            var incVal = int.Parse(increaseValue);
            var incRate = float.Parse(increaseRate);

            Value = new BigDecimal(startValue);
            Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, upgrade);


            var sDelay = float.Parse(startDelayValue);
            var decVal = float.Parse(decreaseDelayValue);
            var decRate = float.Parse(decreaseDelayRate);

            Delay = sDelay;
            Delay = NumberDataUtility.GetIsolationInterest(Delay, decVal, decRate, upgrade);

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