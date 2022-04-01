
namespace SDefence.Perk.Entity
{
    using System.Collections.Generic;
    using Usable;
    using Utility.IO;
    public class PerkUsableEntity : ISavable
    {
        private Dictionary<string, IPerkUsableData> _dic;


        public static PerkUsableEntity Create() => new PerkUsableEntity();

        private PerkUsableEntity()
        {
            _dic = new Dictionary<string, IPerkUsableData>();
        }

        public void AddPerk<T>(int value) where T : IPerkUsableData
        {
            var key = typeof(T).Name;
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, PerkDataUtility.Create<T>(value));
            }
            else
            {
                _dic[key].AddPerk(value);
            }
        }

        public void AddPerk(IPerkUsableData data)
        {
            var key = data.GetType().Name;
            if (!_dic.ContainsKey(key)){
                _dic.Add(key, data);
            }
            else
            {
                _dic[key].AddPerk(data);
            }
        }

        public int GetPerk<T>() where T : IPerkUsableData
        {
            if (_dic.ContainsKey(typeof(T).Name))
            {
                return _dic[typeof(T).Name].GetValue();
            }
            return 0;
        }


        public void SetPerk<T>(int value) where T : IPerkUsableData
        {
            var key = typeof(T).Name;
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, PerkDataUtility.Create<T>(value));
            }
            else
            {
                _dic[key].SetPerk(value);
            }
        }

        public void SetPerk(IPerkUsableData data)
        {
            var key = data.GetType().Name;
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, data);
            }
            else
            {
                _dic[key] = data;
            }
        }


        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            foreach(var key in _dic.Keys)
            {
                data.AddData(_dic[key].SavableKey(), _dic[key].GetSavableData());
            }
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            foreach(var key in data.Children.Keys)
            {
                var perk = PerkDataUtility.Create(key);
                perk.SetSavableData(data.GetValue<SavableData>(key));
                SetPerk(perk);
            }
        }
        public string SavableKey() => typeof(PerkUsableEntity).Name;
    }
}