namespace SDefence.Turret.Entity
{
    using Asset;
    using Durable;
    using Durable.Entity;
    using SDefence.Data;
    using SDefence.Entity;
    using Attack;
    using Enemy;
    using SDefence.Movement;

    public class EnemyEntity : IEntity
    {

        private EnemyData _data;

        private DurableUsableEntity _durableEntity;

        private IAssetUsableData _rewardAssetData;
        private IAttackUsableData _attackData;
        private IMovementUsableData _movementData;

        private LevelWaveData _levelWaveData;


        public string Key => _data.Key;

        //Translate
        public string Name => _data.Key;
        
        //Translate
        public string Description => _data.Key;

        public string IconKey => _data.IconKey;

        public string GraphicObjectKey => _data.GraphicObjectKey;

        public string BulletKey => _data.BulletDataKey;

        public bool IsAttack => _data.IsAttack;

        public TYPE_ENEMY_STYLE TypeEnemyStyle => _data.TypeEnemyStyle;

        public static EnemyEntity Create() => new EnemyEntity();

        private EnemyEntity()
        {
            _durableEntity = DurableUsableEntity.Create();
        }

        public void Initialize(EnemyData data)
        {
            SetData(data);
            _durableEntity.Set(_data.DurableRawDataArray, 0);
        }

        public void SetLevelWave(LevelWaveData data)
        {
            _levelWaveData = data;
        }

        public void CleanUp()
        {
            _data = null;
            _rewardAssetData = null;
            _attackData = null;
            _durableEntity.CleanUp();
        }

        public void SetData(EnemyData data)
        {
            _data = data;
        }

        public DurableBattleEntity GetDurableBattleEntity() => _durableEntity.CreateDurableBattleEntity();


        public IAssetUsableData GetRewardAssetUsableData()
        {
            if(_rewardAssetData == null)
            {
                _rewardAssetData = _data.RewardAssetRawData.GetUsableData(_levelWaveData.GetLevel());
            }
            return _rewardAssetData;
        }

        public IAttackUsableData GetAttackUsableData()
        {
            if(_attackData == null)
            {
                _attackData = _data.AttackRawData.GetUsableData(_levelWaveData.GetLevel());
            }
            return _attackData;
        }

        public string GetDurableUsableData<T>() where T : IDurableUsableData
        {
            //Perk 활용 필요
            return _durableEntity.GetValue<T>();
        }

        public IMovementUsableData GetMovementUsableData()
        {
            if (_movementData == null)
            {
                _movementData = _data.MovementRawData.GetUsableData(_levelWaveData.GetLevel());
            }
            return _movementData;
        }

        public IMovementActionUsableData GetMovementActionUsableData() => _data.MovementRawData.GetActionUsableData();
    }
}