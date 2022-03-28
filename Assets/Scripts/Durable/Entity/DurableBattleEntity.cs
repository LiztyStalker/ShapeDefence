namespace SDefence.Durable.Entity
{
    using System.Collections.Generic;
    using Usable;

    public class DurableBattleEntity
    {
        private Dictionary<string, IDurableUsableData> _dic;

        public static DurableBattleEntity Create() => new DurableBattleEntity();

        private DurableBattleEntity()
        {
            _dic = new Dictionary<string, IDurableUsableData>();
        }

        public void CleanUp()
        {
            _dic.Clear();
            _dic = null;
        }

        private string GetKey(IDurableUsableData dData) => dData.GetType().Name;

        private string GetKey<T>() => typeof(T).Name;

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

        //RecoveryUsableData 변경 필요
        public void Add(IDurableUsableData dData)
        {
            if (HasDurableUsableData<HealthDurableUsableData>())
            {
                _dic[GetKey<HealthDurableUsableData>()].Add(dData);
            }
        }

        //AttackUsableData 변경 필요
        public void Subject(IDurableUsableData dData)
        {
            var value = dData;

            //실드
            if (HasDurableUsableData<ShieldDurableUsableData>())
            {
                //실드 한계치
                if (HasDurableUsableData<LimitDamageShieldDurableUsableData>())
                {
                    var limit = _dic[GetKey<LimitDamageShieldDurableUsableData>()];
                    if (value.Compare(limit) < 0)
                    {
                        value = limit;
                    }
                }

                
                if (_dic[GetKey<ShieldDurableUsableData>()].IsUnderflowZero(value))
                {
                    value.Subject(_dic[GetKey<ShieldDurableUsableData>()]);
                    _dic[GetKey<ShieldDurableUsableData>()].SetZero();
                    //빼고 남은 값 만들기
                }
                else {
                    _dic[GetKey<ShieldDurableUsableData>()].Subject(value);
                    value.SetZero();
                }

                //실드 감소
            }

            if (!value.IsZero)
            {
                //체력
                //방어력 감소
                if (HasDurableUsableData<ArmorDurableUsableData>())
                {
                    //공격력이 더 높음
                    //방어력만큼 감소
                    if (!value.IsUnderflowZero(_dic[GetKey<ArmorDurableUsableData>()]))
                    {
                        value.Subject(_dic[GetKey<ArmorDurableUsableData>()]);
                    }
                    //방어력이 더 높음
                    //공격력 1
                    else
                    {
                        value.Set(1);
                    }
                }

                //체력 적용
                //체력이 높음
                if (!_dic[GetKey<HealthDurableUsableData>()].IsUnderflowZero(value))
                {
                    _dic[GetKey<HealthDurableUsableData>()].Subject(value);
                }
                //공격력이 높음
                else
                {
                    _dic[GetKey<HealthDurableUsableData>()].SetZero();
                }
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

        public bool IsZero(string key)
        {
            if (_dic.ContainsKey(key))
            {
                return _dic[key].IsZero;
            }
            return true;
        }

        private bool HasDurableUsableData<T>()
        {
            string key = typeof(T).Name;
            return _dic.ContainsKey(key);
        }
    }
}