namespace SDefence.Turret.Entity
{
    using Asset;
    using Durable;
    using Durable.Entity;
    using Recovery;
    using Perk.Usable;
    using Perk.Entity;
    using SDefence.Data;
    using Utility.IO;
    using Tech;
    using SDefence.Entity;
    using SDefence.Attack;

    public class TurretEntity : ISavable, IEntity
    {

        #region ##### Parent Savable Data #####

        private TurretData _data;

        #endregion


        #region ##### Savable Data #####

        private UpgradeData _upgradeData;

        #endregion

        private DurableUsableEntity _durableEntity;

        public int _orbitIndex;

        #region ##### Nullable Data #####

        private IRecoveryUsableData _recoveryData;
        private IAssetUsableData _upgradeAssetData;
        private IAttackUsableData _attackData;

        #endregion



        public string Key => _data.Key;

        //Translate
        public string Name => _data.Key;
        
        //Translate
        public string Description => _data.Key;

        public string IconKey => _data.IconKey;

        public string GraphicObjectKey => _data.GraphicObjectKey;

        public string TechDataKey => _data.TechDataKey;

        public int UpgradeValue => _upgradeData.GetValue();

        public float RepairTime => _data.RepairTime;

        public int OrbitIndex => _orbitIndex;

        public string BulletKey => _data.BulletDataKey;

        public static TurretEntity Create() => new TurretEntity();

        private TurretEntity()
        {
            _upgradeData = UpgradeData.Create();
            _durableEntity = DurableUsableEntity.Create();
        }

        public void Initialize(TurretData data, int orbitIndex)
        {
            _orbitIndex = orbitIndex;
            SetData(data);
            _durableEntity.Set(_data.DurableRawDataArray, _upgradeData.GetValue());
        }

        public void CleanUp()
        {
            _data = null;
            _upgradeAssetData = null;
            _upgradeData = null;
            _recoveryData = null;
            _attackData = null;
            _durableEntity.CleanUp();
        }

        public void SetData(TurretData data)
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

        public void UpTech(TurretData data)
        {
            SetData(data);
            ClearUpgrade();
        }

        public DurableBattleEntity GetDurableBattleEntity() => _durableEntity.CreateDurableBattleEntity();


        public IAssetUsableData GetUpgradeData()
        {
            if(_upgradeAssetData == null)
            {
                _upgradeAssetData = _data.UpgradeRawData.GetUsableData(_upgradeData.GetValue());
            }
            return _upgradeAssetData;
        }

        public IAttackUsableData GetAttackUsableData()
        {
            if(_attackData == null)
            {
                _attackData = _data.AttackRawData.GetUsableData(_upgradeData.GetValue());
            }
            return _attackData;
        }

        public string GetDurableUsableData<T>() where T : IDurableUsableData
        {
            //Perk Ȱ�� �ʿ�
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
        public string SavableKey() => typeof(TurretEntity).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            data.AddData("Key", _data.Key);
            data.AddData("Upgrade", _upgradeData.GetValue());
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            var upgrade = data.GetValue<int>("Upgrade");
            _upgradeData.SetValue(upgrade);
        }
        
        #endregion

    }
}