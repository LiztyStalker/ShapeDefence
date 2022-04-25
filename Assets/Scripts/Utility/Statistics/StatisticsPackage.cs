namespace Utility.Statistics
{
    using System.Collections.Generic;
    using System.Numerics;
    using System.Linq;
    using Utility.IO;


    public class StatisticsPackage : ISavable
    {
        private List<StatisticsEntity> _list;

        public static StatisticsPackage Create() => new StatisticsPackage();

        private StatisticsPackage()
        {
            Initialize();
        }

        public void Initialize()
        {
            _list = new List<StatisticsEntity>();
        }

        public void CleanUp()
        {
            _list.Clear();
        }

        public bool IsEmpty() => _list.Count == 0;

        public void Refresh()
        {
            for(int i = 0; i < _list.Count; i++)
            {
                var entity = _list[i];
                OnRefreshStatisticsEvent(entity);
            }
        }

        public void AddStatisticsData<T>(int value = 1) where T : IStatisticsData
        {
            AddStatisticsData<T>(new BigDecimal(value));
        }
        public void AddStatisticsData<T>(BigDecimal value) where T : IStatisticsData
        {
            AddStatisticsData(typeof(T), value);
        }
        public void AddStatisticsData(System.Type type, BigDecimal value)
        {
            if (type != null)
            {
                var iType = type.GetInterface(typeof(IStatisticsData).Name);
                if (iType != null)
                {
                    var index = GetIndex(type);
                    if (index == -1)
                    {
                        _list.Add(StatisticsEntity.Create(type));
                        index = _list.Count - 1;
                    }
                    var entity = _list[index];
                    entity.AddStatisticsData(value);
                    _list[index] = entity;

                    OnRefreshStatisticsEvent(entity);
                }
            }
        }

        public void SetStatisticsData<T>(int value) where T : IStatisticsData
        {
            SetStatisticsData<T>(new BigDecimal(value));
        }

        public void SetStatisticsData<T>(BigDecimal value) where T : IStatisticsData
        {
            SetStatisticsData(typeof(T), value);
        }

        public void SetStatisticsData(System.Type type, BigDecimal value)
        {
            if (type != null)
            {
                var iType = type.GetInterface(typeof(IStatisticsData).Name);
                if (iType != null)
                {

                    var index = GetIndex(type);
                    if (index == -1)
                    {
                        _list.Add(StatisticsEntity.Create(type));
                        index = _list.Count - 1;
                    }
                    var entity = _list[index];
                    entity.SetStatisticsData(value);
                    _list[index] = entity;

                    OnRefreshStatisticsEvent(entity);
                }
            }
        }

        public bool RemoveStatisticsData<T>() where T : IStatisticsData
        {
            return RemoveStatisticsData(typeof(T));
        }

        public bool RemoveStatisticsData(System.Type type)
        {
            if (type != null)
            {
                var iType = type.GetInterface(typeof(IStatisticsData).Name);
                if (iType != null)
                {
                    var index = GetIndex(type);
                    if (index != -1)
                    {
                        _list.RemoveAt(index);
                        return true;
                    }
                }
            }
            return false;
        }

        private int GetIndex(System.Type type) => _list.FindIndex(entity => entity.GetStatisticsType() == type);
        public BigDecimal? GetStatisticsValue<T>() where T : IStatisticsData
        {
            return GetStatisticsValue(typeof(T));
        }

        public BigDecimal? GetStatisticsValue(System.Type type)
        {
            if (type != null)
            {
                var iType = type.GetInterface(typeof(IStatisticsData).Name);
                if (iType != null)
                {
                    var index = GetIndex(type);
                    if (index == -1)
                    {
                        _list.Add(StatisticsEntity.Create(type));
                        index = _list.Count - 1;
                    }
                    return _list[index].GetStatisticsValue();
                }
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning($"GetStatisticsValue {type} �� �ҷ����� ���߽��ϴ�");
#endif
            }
#if UNITY_EDITOR            
            UnityEngine.Debug.LogWarning("GetStatisticsValue type�� �������� �ʾҽ��ϴ�");
#endif
            return null;
        }
        public static System.Type FindType(string key, System.Type classType) => System.Type.GetType($"Utility.Statistics.{key}{classType.Name}");


        #region ##### Listener #####
        private System.Action<StatisticsEntity> _refreshEvent;
        public void SetOnRefreshStatisticsListener(System.Action<StatisticsEntity> act) => _refreshEvent = act;

        private void OnRefreshStatisticsEvent(StatisticsEntity entity)
        {
            _refreshEvent?.Invoke(entity);
        }
        #endregion


        #region ##### Savable #####
        public string SavableKey() => typeof(StatisticsPackage).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            for (int i = 0; i < _list.Count; i++)
            {
                //StatisticsData Remove?
                data.AddData(_list[i].GetStatisticsType().Name, _list[i].GetStatisticsValue().ToString());
            }
            return data;
        }

        public void SetSavableData(SavableData data)
        {

            foreach(var key in data.Children.Keys)
            {
                var type = System.Type.GetType($"Utility.Statistics.{key}"); //StatisticsData Added?
                var child = data.Children[key];
                SetStatisticsData(type, new BigDecimal(child.ToString()));
            }
        }
        #endregion

    }


    #region ##### Utility #####
    public class StatisticsUtility
    {
        private Dictionary<System.Type, string> _dic;

        private static StatisticsUtility _current;

        public static StatisticsUtility Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new StatisticsUtility();
                }
                return _current;
            }
        }

        public string GetTypeToContext(System.Type type)
        {
            if (_dic.ContainsKey(type))
                return _dic[type];
            return null;
        }

        public System.Type[] GetTypes() => _dic.Keys.ToArray();

        public string[] GetValues() => _dic.Values.ToArray();

        public int FindIndex(System.Type type) => _dic.Keys.ToList().FindIndex(t => t == type);

        private StatisticsUtility()
        {
            _dic = new Dictionary<System.Type, string>();

            _dic.Add(typeof(DailyLoginCountStatisticsData), "���� ���� ��");

            _dic.Add(typeof(UpgradeHQStatisticsData), "���� ���� ��");
            _dic.Add(typeof(UpgradeTurretStatisticsData), "��ž ���� ��");
            _dic.Add(typeof(UpTechHQStatisticsData), "���� ���� ��");
            _dic.Add(typeof(UpTechTurretStatisticsData), "��ž ��ũ ��");
            _dic.Add(typeof(DisassembleTurretStatisticsData), "��ž ���� ��");

            _dic.Add(typeof(ArriveLevelStatisticsData), "���� ����");
            _dic.Add(typeof(DestroyEnemyStatisticsData), "�� �ı� ��");
            _dic.Add(typeof(DestroyBossEnemyStatisticsData), "���� �ı� ��");
            _dic.Add(typeof(DestroyMiddleBossEnemyStatisticsData), "�߰� ���� �ı� ��");
            _dic.Add(typeof(DestroySpecialEnemyStatisticsData), "Ư�� ���� �ı� ��");
            _dic.Add(typeof(DestroyThemeBossEnemyStatisticsData), "�׸� ���� �ı� ��");
            _dic.Add(typeof(ClearCountStatisticsData), "�¸� ��");
            _dic.Add(typeof(DefeatCountStatisticsData), "�й� ��");

        }
    }
    #endregion
}