namespace SDefence.Durable.Entity
{
    using System.Collections.Generic;
    using Usable;
    using Recovery;
    using Attack;

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

        public IDurableUsableData GetDurableUsableData<T>() where T : IDurableUsableData
        {
            string key = typeof(T).Name;
            if (_dic.ContainsKey(key))
            {
                return _dic[key];
            }
            return null;
        }

        public float GetRate<T>() where T : IDurableUsableData
        {
            string key = typeof(T).Name;
            if (_dic.ContainsKey(key))
            {
                return _dic[key].GetRate();
            }
            return 0f;
        }

        public void Add(IRecoveryUsableData data)
        {
            var key = data.DurableKey();

            //DurableUsableCase는 내부에서 Over Under 검사 진행
            if (_dic.ContainsKey(key))
            {
                _dic[key].Add(data.CreateUniversalUsableData());
            }
        }


        public void Subject(IAttackUsableData dData)
        {
            var value = dData.CreateUniversalUsableData();

            //실드
            if (HasDurableUsableData<ShieldDurableUsableData>())
            {
                //실드 한계치
                if (HasDurableUsableData<LimitDamageShieldDurableUsableData>())
                {
                    var limit = _dic[GetKey<LimitDamageShieldDurableUsableData>()];
                    if (limit.Value > 0 && value.Compare(limit) < 0)
                    {
                        value = limit.CreateUniversalUsableData();
                    }
                }

                var shield = _dic[GetKey<ShieldDurableUsableData>()];


                if (shield.IsUnderflowZero(value))
                {
                    value.Subject(shield.CreateUniversalUsableData());
                    shield.SetZero();
                    //빼고 남은 값 만들기
                }
                else {
                    shield.Subject(value);
                    value.SetZero();
                }

                //실드 감소
            }

            //UnityEngine.Debug.Log(value.Value);

            if (!value.IsZero)
            {
                //체력
                //방어력 감소
                if (HasDurableUsableData<ArmorDurableUsableData>())
                {
                    var armor = _dic[GetKey<ArmorDurableUsableData>()];

                    //공격력이 더 높음
                    //방어력만큼 감소
                    if (value.Compare(armor) < 0)
                    {
                        value.Subject(armor);
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

        public bool IsZero<T>() where T : IDurableUsableData
        {
            string key = typeof(T).Name;
            if (_dic.ContainsKey(key))
            {
                return _dic[key].IsZero;
            }
            return false;
        }

        private bool HasDurableUsableData<T>()
        {
            string key = typeof(T).Name;
            return _dic.ContainsKey(key);
        }
    }
}