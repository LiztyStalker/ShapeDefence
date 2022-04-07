namespace SDefence.Turret
{
    using UnityEngine;
    using Durable.Raw;
    using Recovery.Raw;
    using Asset.Raw;
    using Utility.ScriptableObjectData;
#if UNITY_EDITOR
    using Generator;
    using Durable.Usable;
    using SDefence.Recovery.Usable;
#endif

    public class TurretData : ScriptableObjectData
    {
        [SerializeField]
        private string _graphicObjectKey;
        [SerializeField]
        private string _bulletDataKey;
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
        private string _attackDataKey;
        [SerializeField]
        private float _repairTime;

        public string GraphicObjectKey => _graphicObjectKey;
        public string TechDataKey => _techDataKey;
        public string AttackDataKey => _attackDataKey;
        public string BulletDataKey => _bulletDataKey;
        public AssetRawData UpgradeRawData => _upgradeRawData;
        public int MaxUpgradeCount => _maxUpgradeCount;
        public DurableRawData[] DurableRawDataArray => _durableRawDataArray;
        public RecoveryRawData RecoveryRawData => _recoveryRawData;
        public float RepairTime => _repairTime;



#if UNITY_EDITOR
        public static TurretData Create() => new TurretData();

        private TurretData()
        {
            Key = "Test";
            IconKey = "Test";
            _bulletDataKey = "Test";
            _techDataKey = "Test";
            _attackDataKey = "Test";
            _graphicObjectKey = "Turret_Test";

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

            _repairTime = 5f;
        }

        public override void AddData(string[] arr) { }

        public override bool HasDataArray() => false;

        public override void SetData(string[] arr)
        {
            Key = arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.Key];

            IconKey = arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IconKey];
            _graphicObjectKey = arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.GraphicObjectKey];

            _durableRawDataArray = new DurableRawData[4];

            var health = DurableRawData.Create();
            health.SetData(typeof(HealthDurableUsableData).FullName, arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartHealthValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseHealthValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseHealthRate]);
            _durableRawDataArray[0] = health;

            var armor = DurableRawData.Create();
            armor.SetData(typeof(ArmorDurableUsableData).FullName, arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartArmorValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseArmorValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseArmorRate]);
            _durableRawDataArray[1] = armor;

            var shield = DurableRawData.Create();
            shield.SetData(typeof(ShieldDurableUsableData).FullName, arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartShieldValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseShieldValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseShieldRate]);
            _durableRawDataArray[2] = shield;

            var limShield = DurableRawData.Create();
            limShield.SetData(typeof(LimitDamageShieldDurableUsableData).FullName, arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartFloorShieldValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.DecreaseFloorShieldValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.DecreaseFloorShieldRate]);
            _durableRawDataArray[3] = limShield;


            var recovery = RecoveryRawData.Create();
            recovery.SetData(typeof(HealthRecoveryUsableData).FullName, arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartRecoveryShieldValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseRecoveryShieldValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseRecoveryShieldRate]);
            _recoveryRawData = recovery;

            var asset = AssetRawData.Create();
            asset.SetData(arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.TypeAssetData], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartUpgradeValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseUpgradeValue], arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseUpgradeRate]);
            _upgradeRawData = asset;

            _maxUpgradeCount = int.Parse(arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.MaxUpgradeCount]);

            _repairTime = int.Parse(arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.RepairTime]);

            _bulletDataKey = arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.BulletDataKey];

            _attackDataKey = arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.AttackDataKey];

            //_techDataKey = arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey];
        }

#endif
    }
}