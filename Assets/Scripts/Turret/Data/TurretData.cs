namespace SDefence.Turret
{
    using UnityEngine;
    using Durable.Raw;
    using Recovery.Raw;
    using Asset.Raw;
    using Attack.Raw;
    using Utility.ScriptableObjectData;
    using Durable.Usable;
    using Recovery.Usable;
    using Tech;
#if UNITY_EDITOR
    using Generator;
    using System.Collections.Generic;
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
        private TechRawData _techRawData;
        [SerializeField]
        private AttackRawData _attackData;
        [SerializeField]
        private float _repairTime;

        public string GraphicObjectKey => _graphicObjectKey;
        public string TechDataKey => _techDataKey;
        public TechRawData TechRawData => _techRawData;
        public AttackRawData AttackRawData => _attackData;
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
            _graphicObjectKey = "Turret_Test";

            _durableRawDataArray = new DurableRawData[4];
            var health = DurableRawData.Create();
            health.SetData("SDefence.Durable.Usable.HealthDurableUsableData", "100", "1", "0.1");
            _durableRawDataArray[0] = health;
            var armor = DurableRawData.Create();
            armor.SetData("SDefence.Durable.Usable.ArmorDurableUsableData", "1", "0", "0.1");
            _durableRawDataArray[1] = armor;
            var shield = DurableRawData.Create();
            shield.SetData("SDefence.Durable.Usable.ShieldDurableUsableData", "100", "0", "0.1");
            _durableRawDataArray[2] = shield;
            var limShield = DurableRawData.Create();
            limShield.SetData("SDefence.Durable.Usable.LimitDamageShieldDurableUsableData", "50", "0", "0.1");
            _durableRawDataArray[3] = limShield;

            var recovery = RecoveryRawData.Create();
            _recoveryRawData = recovery;

            var asset = AssetRawData.Create();
            _upgradeRawData = asset;

            _maxUpgradeCount = 10;

            _attackData = AttackRawData.Create();
            _attackData.SetDelay("0.25", "0", "0.01");

            _repairTime = 5f;

            _techRawData = TechRawData.Create("Turret");
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

            var attackData = AttackRawData.Create();

            attackData.SetAttack(
                    arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartAttackValue],
                    arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseAttackValue],
                    arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.IncreaseAttackRate]
                    );
            attackData.SetDelay(
                    arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartAttackDelayValue],
                    arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackDelayValue],
                    arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackDelayRate]
                );
            attackData.SetRange(
                arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.StartAttackRangeValue],
                arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackRangeValue],
                arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackRangeRate]
            );

            _attackData = attackData;

            if((int)TurretDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey < arr.Length)
                _techDataKey = arr[(int)TurretDataGenerator.TYPE_SHEET_COLUMNS.TechDataKey];
        }

        public void SetTechRawData(TechRawData raw)
        {
            _techRawData = raw;
        }

        public void ClearTechRawData() => _techRawData = null;
#endif
    }
}