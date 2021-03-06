namespace SDefence.HQ
{
    using UnityEngine;
    using Durable.Raw;
    using Recovery.Raw;
    using Asset.Raw;
    using Utility.ScriptableObjectData;
    using Tech;
    using SDefence.Recovery.Usable;
    using Durable.Usable;
#if UNITY_EDITOR
    using Generator;
#endif

    public class HQData : ScriptableObjectData
    {
        [SerializeField]
        private string _graphicObjectKey;
        [SerializeField]
        private DurableRawData[] _durableRawDataArray;
        [SerializeField]
        private RecoveryRawData _recoveryRawData;
        [SerializeField]
        private AssetRawData _upgradeRawData;
        [SerializeField]
        private int _maxUpgradeCount;
        [SerializeField]
        private string _techDataKey;
        [SerializeField]
        private TechRawData _techRawData;
        [SerializeField]
        private int _turretCount;

        public string GraphicObjectKey => _graphicObjectKey;
        public string TechDataKey => _techDataKey;
        public TechRawData TechRawData => _techRawData;
        public AssetRawData UpgradeRawData => _upgradeRawData;
        public int MaxUpgradeCount => _maxUpgradeCount;
        public DurableRawData[] DurableRawDataArray => _durableRawDataArray;
        public RecoveryRawData RecoveryRawData => _recoveryRawData;
        public int TurretCount => _turretCount;



#if UNITY_EDITOR
        public static HQData Create() => new HQData();

        private HQData()
        {
            Key = "Test";
            IconKey = "Test";
            _graphicObjectKey = "HQ_Test";

            _durableRawDataArray = new DurableRawData[4];
            var health = DurableRawData.Create();
            _durableRawDataArray[0] = health;
            var armor = DurableRawData.Create();
            _durableRawDataArray[1] = armor;
            var shield = DurableRawData.Create();
            _durableRawDataArray[2] = shield;
            var limShield = DurableRawData.Create();
            _durableRawDataArray[3] = limShield;

            var recovery = RecoveryRawData.Create();
            _recoveryRawData = recovery;

            var asset = AssetRawData.Create();
            _upgradeRawData = asset;

            _maxUpgradeCount = 10;

            _turretCount = 1;

            _techRawData = TechRawData.Create("HQ");
        }

        public override void AddData(string[] arr) { }

        public override bool HasDataArray() => false;

        public override void SetData(string[] arr)
        {
            Key = arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.Key];

            IconKey = arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IconKey];
            _graphicObjectKey = arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.ObjectKey];

            _durableRawDataArray = new DurableRawData[4];

            var health = DurableRawData.Create();
            health.SetData(typeof(HealthDurableUsableData).FullName, arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.StartHealthValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseHealthValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseHealthRate]);
            _durableRawDataArray[0] = health;

            var armor = DurableRawData.Create();
            armor.SetData(typeof(ArmorDurableUsableData).FullName, arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.StartArmorValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseArmorValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseArmorRate]);
            _durableRawDataArray[1] = armor;

            var shield = DurableRawData.Create();
            shield.SetData(typeof(ShieldDurableUsableData).FullName, arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.StartShieldValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseShieldValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseShieldRate]);
            _durableRawDataArray[2] = shield;

            var limShield = DurableRawData.Create();
            limShield.SetData(typeof(LimitDamageShieldDurableUsableData).FullName, arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.StartFloorShieldValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.DecreaseFloorShieldValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.DecreaseFloorShieldRate]);
            _durableRawDataArray[3] = limShield;


            var recovery = RecoveryRawData.Create();
            recovery.SetData(typeof(HealthRecoveryUsableData).FullName, arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.StartRecoveryShieldValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseRecoveryShieldValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseRecoveryShieldRate]);
            _recoveryRawData = recovery;

            var asset = AssetRawData.Create();
            asset.SetData(arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.TypeAssetData], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.StartUpgradeValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseUpgradeValue], arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.IncreaseUpgradeRate]);
            _upgradeRawData = asset;

            _maxUpgradeCount = int.Parse(arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.MaxUpgradeCount]);

            _turretCount = int.Parse(arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.TurretCount]);

            if (arr.Length > (int)HQDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey)
            {
                _techDataKey = arr[(int)HQDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey];
            }
            else
            {
                _techDataKey = null;
            }
        }

        public void SetTechRawData(TechRawData raw)
        {
            _techRawData = raw;
        }

        

#endif
    }
}