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
                UnityEngine.Debug.LogWarning($"GetStatisticsValue {type} 을 불러오지 못했습니다");
#endif
            }
#if UNITY_EDITOR            
            UnityEngine.Debug.LogWarning("GetStatisticsValue type을 지정하지 않았습니다");
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
                UnityEngine.Debug.Log(key);
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

            _dic.Add(typeof(CreateUnitStatisticsData), "유닛 생산 수");
            
            _dic.Add(typeof(DestroyUnitStatisticsData), "유닛 파괴된 수");

            _dic.Add(typeof(AccumulativelyGoldUsedAssetStatisticsData), "누적 골드 소비");
            _dic.Add(typeof(AccumulativelyResourceUsedAssetStatisticsData), "누적 자원 소비");
            _dic.Add(typeof(AccumulativelyMeteoriteUsedAssetStatisticsData), "누적 운철 소비");
            _dic.Add(typeof(AccumulativelyResearchUsedAssetStatisticsData), "누적 연구 소비");

            _dic.Add(typeof(AccumulativelyGoldGetAssetStatisticsData), "누적 골드 획득");
            _dic.Add(typeof(AccumulativelyResourceGetAssetStatisticsData), "누적 자원 획득");
            _dic.Add(typeof(AccumulativelyMeteoriteGetAssetStatisticsData), "누적 운철 획득");
            _dic.Add(typeof(AccumulativelyResearchGetAssetStatisticsData), "누적 연구 획득");

            _dic.Add(typeof(TechUnitStatisticsData), "유닛 테크 진행 수");

            _dic.Add(typeof(UpgradeUnitStatisticsData), "유닛 업글 진행 수");

            _dic.Add(typeof(DestroyEnemyStatisticsData), "적 파괴 수");
            _dic.Add(typeof(DestroyBossStatisticsData), "보스 파괴 수");
            _dic.Add(typeof(DestroyThemeBossStatisticsData), "테마 보스 파괴 수");

            _dic.Add(typeof(ArrivedLevelStatisticsData), "누적 레벨 도달");
            _dic.Add(typeof(MaxArrivedLevelStatisticsData), "최대 레벨 도달");

            _dic.Add(typeof(ExpandWorkshopLineStatisticsData), "제작소 라인 증축 수");

            _dic.Add(typeof(UpgradeVillageStatisticsData), "마을 업글 진행 수");
            _dic.Add(typeof(TechVillageStatisticsData), "마을 테크 진행 수");

            _dic.Add(typeof(UpgradeSmithyStatisticsData), "대장간 업글 진행 수");
            _dic.Add(typeof(TechSmithyStatisticsData), "대장간 테크 진행 수");

            _dic.Add(typeof(SuccessResearchStatisticsData), "연구 진행 수 (미개발)");
            //_dic.Add(typeof(UpgradeCommanderStatisticsData), "지휘관 업글 진행 수");

            _dic.Add(typeof(AchievedDailyStatisticsData), "누적 일일 퀘스트 진행 수");
            _dic.Add(typeof(AchievedWeeklyStatisticsData), "누적 주간 퀘스트 진행 수");
            _dic.Add(typeof(AchievedChallengeStatisticsData), "누적 도전 퀘스트 진행 수");
            _dic.Add(typeof(AchievedGoalStatisticsData), "누적 목표 퀘스트 진행 수");

            //_dic.Add(typeof(SuccessExpeditionStatisticsData), "누적 원정 진행 수 (미개발)");



        }
    }
    #endregion
}