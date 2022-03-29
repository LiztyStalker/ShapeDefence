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

        public void Add(IRecoveryUsableData data)
        {
            var key = data.DurableKey();

            //DurableUsableCase�� ���ο��� Over Under �˻� ����
            if (_dic.ContainsKey(key))
            {
                _dic[key].Add(data.CreateUniversalUsableData());
            }
        }


        public void Subject(IAttackUsableData dData)
        {
            var value = dData.CreateUniversalUsableData();

            //�ǵ�
            if (HasDurableUsableData<ShieldDurableUsableData>())
            {
                //�ǵ� �Ѱ�ġ
                if (HasDurableUsableData<LimitDamageShieldDurableUsableData>())
                {
                    var limit = _dic[GetKey<LimitDamageShieldDurableUsableData>()];
                    if (value.Compare(limit) < 0)
                    {
                        value = limit.CreateUniversalUsableData();
                    }
                }

                var shield = _dic[GetKey<ShieldDurableUsableData>()];


                if (shield.IsUnderflowZero(value))
                {
                    value.Subject(shield.CreateUniversalUsableData());
                    shield.SetZero();
                    //���� ���� �� �����
                }
                else {
                    shield.Subject(value);
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
                    var armor = _dic[GetKey<ArmorDurableUsableData>()];

                    //���ݷ��� �� ����
                    //���¸�ŭ ����
                    if (value.Compare(armor) < 0)
                    {
                        value.Subject(armor);
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