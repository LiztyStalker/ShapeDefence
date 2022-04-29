namespace SDefence.Enemy
{
    using UnityEngine;
    using Durable.Raw;
    using Asset.Raw;
    using Utility.ScriptableObjectData;
    using Durable.Usable;
    using Attack.Raw;
    using Movement.Raw;
#if UNITY_EDITOR
    using Generator;
#endif

    public enum TYPE_ENEMY_STYLE
    {
        Normal,
        Special,
        NormalBoss,
        SpecialBoss,
        MiddleBoss,
        ThemeBoss,
    }

    public class EnemyData : ScriptableObjectData
    {
        [SerializeField]
        private string _graphicObjectKey;
        [SerializeField]
        private TYPE_ENEMY_STYLE _typeEnemyStyle;
        [SerializeField]
        private DurableRawData[] _durableRawDataArray;
        [SerializeField]
        private AssetRawData _rewardAssetRawData;
        [SerializeField]
        private bool _isAttack = false;
        [SerializeField]
        private AttackRawData _attackRawData;
        [SerializeField]
        private string _bulletDataKey;
        [SerializeField]
        private MovementRawData _movementRawData;

        public TYPE_ENEMY_STYLE TypeEnemyStyle => _typeEnemyStyle;
        public string GraphicObjectKey => _graphicObjectKey;
        public AssetRawData RewardAssetRawData => _rewardAssetRawData;
        public DurableRawData[] DurableRawDataArray => _durableRawDataArray;
        public bool IsAttack => _isAttack;
        public AttackRawData AttackRawData => _attackRawData;
        public string BulletDataKey => _bulletDataKey;
        public MovementRawData MovementRawData => _movementRawData;


#if UNITY_EDITOR
        public static EnemyData Create() => new EnemyData();

        private EnemyData()
        {
            Key = "Test";
            IconKey = "Test";
            _graphicObjectKey = "Enemy_Test";
            _bulletDataKey = "Bullet_Test";

            _durableRawDataArray = new DurableRawData[4];
            var health = DurableRawData.Create();
            _durableRawDataArray[0] = health;
            var armor = DurableRawData.Create();
            _durableRawDataArray[1] = armor;
            var shield = DurableRawData.Create();
            _durableRawDataArray[2] = shield;
            var limShield = DurableRawData.Create();
            _durableRawDataArray[3] = limShield;

            _attackRawData = AttackRawData.Create();
            _movementRawData = MovementRawData.Create();

            _isAttack = false;
        }

        public override void AddData(string[] arr) { }

        public override bool HasDataArray() => false;

        public override void SetData(string[] arr)
        {
            Key = arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.Key];

            IconKey = arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IconKey];
            _graphicObjectKey = arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.GraphicObjectKey];

            _typeEnemyStyle = (TYPE_ENEMY_STYLE)System.Enum.Parse(typeof(TYPE_ENEMY_STYLE), arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.TypeEnemy]);

            _durableRawDataArray = new DurableRawData[4];

            var health = DurableRawData.Create();
            health.SetData(typeof(HealthDurableUsableData).FullName, arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartHealthValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseHealthValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseHealthRate]);
            _durableRawDataArray[0] = health;

            var armor = DurableRawData.Create();
            armor.SetData(typeof(ArmorDurableUsableData).FullName, arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartArmorValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseArmorValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseArmorRate]);
            _durableRawDataArray[1] = armor;

            var shield = DurableRawData.Create();
            shield.SetData(typeof(ShieldDurableUsableData).FullName, arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartShieldValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseShieldValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseShieldRate]);
            _durableRawDataArray[2] = shield;

            var limShield = DurableRawData.Create();
            limShield.SetData(typeof(LimitDamageShieldDurableUsableData).FullName, arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartFloorShieldValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.DecreaseFloorShieldValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.DecreaseFloorShieldRate]);
            _durableRawDataArray[3] = limShield;

            var asset = AssetRawData.Create();
            asset.SetData(arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.TypeRewardAssetData], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartRewardAssetValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseRewardAssetValue], arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseRewardAssetRate]);
            _rewardAssetRawData = asset;

            _isAttack = bool.Parse(arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IsAttack]);

            var attack = AttackRawData.Create();

            attack.SetAttack(
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartAttackValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseAttackValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseAttackRate]
                );
            attack.SetDelay(
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartAttackDelayValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackDelayValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackDelayRate]
                );
            attack.SetRange(
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartAttackRangeValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackRangeValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.DecreaseAttackRangeRate]
                );

            _attackRawData = attack;

            var movement = MovementRawData.Create();
            movement.SetData(
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.StartMovementValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseMovementValue],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.IncreaseMovementRate]
                );
            movement.SetData(
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.TypeMovement],
                arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.Accuracy]
                );
            _movementRawData = movement;

            _bulletDataKey = arr[(int)EnemyDataGenerator.TYPE_SHEET_COLUMNS.BulletDataKey];

        }

#endif
    }
}