namespace Utility.Statistics
{
    using System.Numerics;
    using Utility.IO;

    public struct StatisticsEntity
    {
        private System.Type _type;
        private BigDecimal _value;

        private StatisticsEntity(System.Type type)
        {
            _type = type;
            _value = new BigDecimal();
        }

        internal void AddStatisticsData(BigDecimal value)
        {
            _value += value;
        }
        internal void SetStatisticsData(BigDecimal value)
        {
            _value = value;
        }
        public BigDecimal GetStatisticsValue() => _value;
        public System.Type GetStatisticsType() => _type;

        internal static StatisticsEntity Create<T>() where T : IStatisticsData
        {
            return new StatisticsEntity(typeof(T));
        }
        
        public static StatisticsEntity Create(System.Type type)
        {
            return new StatisticsEntity(type);
        }

        //#region ##### StorableData #####

      
        //public string SavableKey() => _type.Name;

        //public SavableData GetSavableData()
        //{
        //    var data = SavableData.Create();
        //    data.AddData("Type", _type.AssemblyQualifiedName);
        //    data.AddData("Value", _value.ToString());
        //    return data;
        //}

        //public void SetSavableData(SavableData data)
        //{
        //    var storableData = data;

        //    var type = System.Type.GetType(storableData.GetValue<string>("Type"));
        //    _type = type;

        //    var bigInt = new BigInteger();
        //    var str = storableData.GetValue<string>("Value");

        //    int index = 0;
        //    while (true)
        //    {
        //        var sub = (str.Length > (index + int.MaxValue.ToString().Length - 1)) ?
        //            (str.Substring(index, int.MaxValue.ToString().Length - 1)) :
        //            (str.Substring(index));

        //        var val = int.Parse(sub);

        //        bigInt *= BigInteger.Pow(10, sub.Length);
        //        bigInt += val;
        //        index += sub.Length;

        //        if (index >= str.Length)
        //        {
        //            break;
        //        }
        //    }
        //    _value = bigInt;
        //}
        //#endregion
    }
}