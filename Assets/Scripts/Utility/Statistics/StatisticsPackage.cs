namespace Utility.Statistics
{
    using System.Collections.Generic;
    using System.Numerics;
    using System.Linq;
    using Utility.IO;


    public class StatisticsPackage : ISavable
    {
        private List<StatisticsEntity> _list;



        public static StatisticsPackage Create()
        {
            return new StatisticsPackage();
        }

        public void Initialize()
        {
            _list = new List<StatisticsEntity>();
        }

        public void CleanUp()
        {
            _list.Clear();
        }

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
                data.AddData(_list[i].GetStatisticsType().Name, _list[i].GetStatisticsValue().ToString());
            }
            return data;
        }

        public void SetSavableData(SavableData data)
        {

            foreach(var key in data.Children.Keys)
            {
                var type = System.Type.GetType($"Utility.Statistics.{key}"); //StatisticsData
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

            _dic.Add(typeof(CreateUnitStatisticsData), "���� ���� ��");
            
            _dic.Add(typeof(DestroyUnitStatisticsData), "���� �ı��� ��");

            _dic.Add(typeof(AccumulativelyGoldUsedAssetStatisticsData), "���� ��� �Һ�");
            _dic.Add(typeof(AccumulativelyResourceUsedAssetStatisticsData), "���� �ڿ� �Һ�");
            _dic.Add(typeof(AccumulativelyMeteoriteUsedAssetStatisticsData), "���� ��ö �Һ�");
            _dic.Add(typeof(AccumulativelyResearchUsedAssetStatisticsData), "���� ���� �Һ�");

            _dic.Add(typeof(AccumulativelyGoldGetAssetStatisticsData), "���� ��� ȹ��");
            _dic.Add(typeof(AccumulativelyResourceGetAssetStatisticsData), "���� �ڿ� ȹ��");
            _dic.Add(typeof(AccumulativelyMeteoriteGetAssetStatisticsData), "���� ��ö ȹ��");
            _dic.Add(typeof(AccumulativelyResearchGetAssetStatisticsData), "���� ���� ȹ��");

            _dic.Add(typeof(TechUnitStatisticsData), "���� ��ũ ���� ��");

            _dic.Add(typeof(UpgradeUnitStatisticsData), "���� ���� ���� ��");

            _dic.Add(typeof(DestroyEnemyStatisticsData), "�� �ı� ��");
            _dic.Add(typeof(DestroyBossStatisticsData), "���� �ı� ��");
            _dic.Add(typeof(DestroyThemeBossStatisticsData), "�׸� ���� �ı� ��");

            _dic.Add(typeof(ArrivedLevelStatisticsData), "���� ���� ����");
            _dic.Add(typeof(MaxArrivedLevelStatisticsData), "�ִ� ���� ����");

            _dic.Add(typeof(ExpandWorkshopLineStatisticsData), "���ۼ� ���� ���� ��");

            _dic.Add(typeof(UpgradeVillageStatisticsData), "���� ���� ���� ��");
            _dic.Add(typeof(TechVillageStatisticsData), "���� ��ũ ���� ��");

            _dic.Add(typeof(UpgradeSmithyStatisticsData), "���尣 ���� ���� ��");
            _dic.Add(typeof(TechSmithyStatisticsData), "���尣 ��ũ ���� ��");

            _dic.Add(typeof(SuccessResearchStatisticsData), "���� ���� �� (�̰���)");
            //_dic.Add(typeof(UpgradeCommanderStatisticsData), "���ְ� ���� ���� ��");

            _dic.Add(typeof(AchievedDailyStatisticsData), "���� ���� ����Ʈ ���� ��");
            _dic.Add(typeof(AchievedWeeklyStatisticsData), "���� �ְ� ����Ʈ ���� ��");
            _dic.Add(typeof(AchievedChallengeStatisticsData), "���� ���� ����Ʈ ���� ��");
            _dic.Add(typeof(AchievedGoalStatisticsData), "���� ��ǥ ����Ʈ ���� ��");

            //_dic.Add(typeof(SuccessExpeditionStatisticsData), "���� ���� ���� �� (�̰���)");



        }
    }
    #endregion
}