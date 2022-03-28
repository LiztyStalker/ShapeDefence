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

        //RecoveryUsableData ���� �ʿ�
        public void Add(IDurableUsableData dData)
        {
            if (HasDurableUsableData<HealthDurableUsableData>())
            {
                _dic[GetKey<HealthDurableUsableData>()].Add(dData);
            }
        }

        //AttackUsableData ���� �ʿ�
        public void Subject(IDurableUsableData dData)
        {
            var value = dData;

            //�ǵ�
            if (HasDurableUsableData<ShieldDurableUsableData>())
            {
                //�ǵ� �Ѱ�ġ
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
                    //���� ���� �� �����
                }
                else {
                    _dic[GetKey<ShieldDurableUsableData>()].Subject(value);
                    value.SetZero();
                }

                //�ǵ� ����
            }

            if (!value.IsZero)
            {
                //ü��
                //���� ����
                if (HasDurableUsableData<ArmorDurableUsableData>())
                {
                    //���ݷ��� �� ����
                    //���¸�ŭ ����
                    if (!value.IsUnderflowZero(_dic[GetKey<ArmorDurableUsableData>()]))
                    {
                        value.Subject(_dic[GetKey<ArmorDurableUsableData>()]);
                    }
                    //������ �� ����
                    //���ݷ� 1
                    else
                    {
                        value.Set(1);
                    }
                }

                //ü�� ����
                //ü���� ����
                if (!_dic[GetKey<HealthDurableUsableData>()].IsUnderflowZero(value))
                {
                    _dic[GetKey<HealthDurableUsableData>()].Subject(value);
                }
                //���ݷ��� ����
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