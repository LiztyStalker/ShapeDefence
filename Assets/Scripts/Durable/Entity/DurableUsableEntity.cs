namespace SDefence.Durable.Entity
{
    using Raw;
    using System.Collections.Generic;
    using Usable;
    public class DurableUsableEntity
    {
        private Dictionary<string, IDurableUsableData> _dic;

        public static DurableUsableEntity Create() => new DurableUsableEntity();

        private DurableUsableEntity()
        {
            _dic = new Dictionary<string, IDurableUsableData>();
        }

        public void CleanUp()
        {
            _dic.Clear();
            _dic = null;
        }

        private string GetKey(IDurableUsableData dData) => dData.GetType().Name;

        public void Set(DurableRawData[] arr, int upgrade)
        {
            for(int i = 0; i < arr.Length; i++)
            {
                Set(arr[i].GetUsableData(upgrade));
            }
        }
        public void Set(IDurableUsableData dData)
        {
            string key = GetKey(dData);
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, dData);
            }
            else
            {
                _dic[key] = dData;
            }
        }


        public string GetValue<T>(string format = null) where T : IDurableUsableData
        {
            string key = typeof(T).Name;
            if (_dic.ContainsKey(key))
            {
                return _dic[key].ToString(format);
            }
#if UNITY_EDITOR
            return "-Empty-";
#else
            return "";
#endif
        }

        public DurableBattleEntity CreateDurableBattleEntity()
        {
            var entity = DurableBattleEntity.Create();
            foreach(var value in _dic.Values)
            {
                if (value is ILimitedDurable)
                {
                    var caseData = ((ILimitedDurable)value).GetDurableUsableCase();
                    entity.Set(caseData);
                }
                else
                {
                    entity.Set(value.Clone());
                }
            }
            return entity;
        }
    }
}