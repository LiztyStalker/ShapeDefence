namespace SDefence.Asset.Usable
{
    using System.Numerics;
    using Utility.IO;
    using Utility.Number;

    public abstract class AbstractAssetUsableData : IAssetUsableData
    {
        private BigDecimal _value;
        public BigDecimal Value { get => _value; protected set => _value = value; }


        public bool IsZero => Value.IsZero;
        public void SetZero() => Value = 0;


        public virtual void Add(int value) => Value += value;
        public virtual void Add(IAssetUsableData value) => Value += ((AbstractAssetUsableData)value).Value;
        public virtual void Subject(int value) => Value -= value;
        public virtual void Subject(IAssetUsableData value) => Value -= ((AbstractAssetUsableData)value).Value;     
        public virtual void Set(int value) => Value = value;
        public virtual void Set(IAssetUsableData value) => Value = ((AbstractAssetUsableData)value).Value;
        public void SetData(string startValue, string increaseValue, string increaseRate, int upgrade)
        {
            var sVal = long.Parse(startValue);
            var incVal = int.Parse(increaseValue);
            var incRate = float.Parse(increaseRate);

            Value = new BigDecimal(sVal);
            Value = NumberDataUtility.GetIsolationInterest(Value, incVal, incRate, upgrade);
        }


        public int Compare(IAssetUsableData value)
        {
            if (Value - ((AbstractAssetUsableData)value).Value > 0) return -1;
            else if (Value - ((AbstractAssetUsableData)value).Value < 0) return 1;
            return 0;
        }

        protected static T Create<T>() where T : IAssetUsableData
        {
            return System.Activator.CreateInstance<T>();
        }

        public static IAssetUsableData Create(string type)
        {
            var classType = System.Type.GetType(type);
            if (classType != null)
                return Create(classType);
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning($"{type} Class is not Found");
#endif
            return null;
        }

        public static IAssetUsableData Create(System.Type type)
        {
            return (IAssetUsableData)System.Activator.CreateInstance(type);
        }


        public string ToString(string format) => Value.ToString(format);
        public override string ToString() => Value.ToString();


        public IAssetUsableData Clone()
        {
            var data = Create(GetType());
            data.Set(this);
            return data;
        }



        #region ##### Savable #####
        public abstract string SavableKey();
        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            data.AddData("value", Value.Value);
            data.AddData("decimalPoint", Value.DecimalPoint);
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            _value = new BigDecimal((BigInteger)data.Children["value"], (byte)data.Children["decimalPoint"]);
        }

        #endregion


    }
}