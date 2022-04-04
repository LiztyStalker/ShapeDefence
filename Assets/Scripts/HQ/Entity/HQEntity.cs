namespace SDefence.HQ.Entity
{
    using Asset;
    using Durable;
    using Durable.Entity;
    using Recovery;
    using Perk.Usable;
    using Perk.Entity;
    using SDefence.Data;
    using Utility.IO;
    using Tech.Raw;
    using SDefence.Entity;

    public class HQEntity : ISavable, IEntity
    {

        #region ##### Parent Savable Data #####

        private HQData _data;

        #endregion


        #region ##### Savable Data #####

        private PerkUsableEntity _perkEntity;
        private UpgradeData _upgradeData;

        #endregion

        private DurableUsableEntity _durableEntity;

        #region ##### Nullable Data #####

        private IRecoveryUsableData _recoveryData;
        private IAssetUsableData _upgradeAssetData;

        #endregion



        public string Key => _data.Key;

        //Translate
        public string Name => _data.Key;
        
        //Translate
        public string Description => _data.Key;

        public int UpgradeValue => _upgradeData.GetValue();

        public static HQEntity Create() => new HQEntity();

        private HQEntity()
        {
            _upgradeData = UpgradeData.Create();
            _durableEntity = DurableUsableEntity.Create();
            _perkEntity = PerkUsableEntity.Create();
        }

        public void Initialize(HQData data)
        {
            SetData(data);
            _durableEntity.Set(_data.DurableRawDataArray, _upgradeData.GetValue());
        }
        
        public void SetData(HQData data)
        {
            _data = data;
        }

        public void Upgrade()
        {
            _upgradeData.IncreaseNumber();
            _upgradeAssetData = null;
            _recoveryData = null;

            _durableEntity.Set(_data.DurableRawDataArray, _upgradeData.GetValue());
        }
        public bool IsMaxUpgrade() => _upgradeData.GetValue() == _data.MaxUpgradeCount;

        public void ClearUpgrade()
        {
            _upgradeData.CleanUp();
            _upgradeAssetData = null;
            _recoveryData = null;

            _durableEntity.Set(_data.DurableRawDataArray, _upgradeData.GetValue());
        }

        public void UpTech(HQData data)
        {
            SetData(data);
            ClearUpgrade();
        }

        public TechRawData Tech() => _data.TechRawData;


        public DurableBattleEntity GetDurableBattleEntity() => _durableEntity.CreateDurableBattleEntity();



        public void AddPerk<T>(int value = 1) where T : IPerkUsableData => _perkEntity.AddPerk<T>(value);
        public int GetPerk<T>() where T : IPerkUsableData => _perkEntity.GetPerk<T>();






        public IAssetUsableData GetUpgradeData()
        {
            if(_upgradeAssetData == null)
            {
                _upgradeAssetData = _data.UpgradeRawData.GetUsableData(_upgradeData.GetValue());
            }
            return _upgradeAssetData;
        }

        public string GetDurableUsableData<T>() where T : IDurableUsableData
        {
            //Perk 활용 필요
            return _durableEntity.GetValue<T>();
        }
        public IRecoveryUsableData GetRecoveryUsableData<T>() where T : IRecoveryUsableData
        {
            if(_recoveryData == null)
            {
                _recoveryData = _data.RecoveryRawData.GetUsableData(_upgradeData.GetValue());
            }

            return _recoveryData;
        }



        #region ##### Savable #####
        public string SavableKey() => typeof(HQEntity).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            data.AddData("Key", _data.Key);
            data.AddData(_perkEntity.SavableKey(), _perkEntity.GetSavableData());
            data.AddData("Upgrade", _upgradeData.GetValue());
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            _perkEntity.SetSavableData(data.GetValue<SavableData>(_perkEntity.SavableKey()));
            var upgrade = data.GetValue<int>("Upgrade");
            _upgradeData.SetValue(upgrade);
        }
        
        #endregion

    }
}