namespace Utility.Bullet.Data
{
    using UnityEngine;
    using Storage;
    using Effect.Data;
    using ScriptableObjectData;
    using SDefence.Attack.Raw;
    using SDefence.Movement.Raw;
    using SDefence.Attack.Usable;
    using SDefence.Movement;

    [CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/BulletData")]
    public class BulletData : ScriptableObjectData
    {
        public enum TYPE_BULLET_ACTION { Move, Curve, Drop, Direct }

        [SerializeField]
        private string _graphicObjectKey;

        [SerializeField]
        private AttackActionRawData _attackActionData;

        [SerializeField]
        private MovementRawData _movementData;

        [SerializeField]
        private bool _isRotate = false;

        [SerializeField]
        private string _activeEffectDataKey;

        [SerializeField]
        private string _destroyEffectDataKey;

        [SerializeField]
        private string _activeEffectSfxKey;

        [SerializeField]
        private string _destroyEffectSfxKey;




        public string GraphicObjectKey => _graphicObjectKey;
        public AttackActionUsableData GetAttackActionUsableData() => _attackActionData.GetUsableData();
        public IMovementActionUsableData GetMovementActionUsableData() => _movementData.GetActionUsableData();
        public IMovementUsableData GetMovementUsableData() => _movementData.GetUsableData();
        public bool IsRotate => _isRotate;
        public string ActiveEffectDataKey => _activeEffectDataKey;
        public string DestroyEffectDataKey => _destroyEffectDataKey;
        public string ActiveEffectSfxKey => _activeEffectSfxKey;
        public string DestroyEffectSfxKey => _destroyEffectSfxKey;



#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public static BulletData CreateTest()
        {
            return new BulletData("Data@Bullet");
        }

        public override void SetData(string[] arr)
        {
            Key = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.Key];

            name = $"{typeof(BulletData).Name}_{Key}";

            _graphicObjectKey = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.GraphicObjectKey];

            var attack = AttackActionRawData.Create();
            attack.SetData(
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.Range],
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.AttackCount],
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.AttackDelayTime],
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.IsOverlap]
                );
            _attackActionData = attack;

            var movement = MovementRawData.Create();
            movement.SetData(
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.StartMovementValue],
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.IncreaseMovementValue],
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.IncreaseMovementRate]
                );

            movement.SetData(
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.TypeMovement],
                arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.Accuracy]
                );

            _movementData = movement;

            _isRotate = bool.Parse(arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.IsRotate]);

            _activeEffectDataKey = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.ActiveEffectKey];
            _activeEffectSfxKey = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.ActiveSfxKey];
            _destroyEffectDataKey = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.DestroyEffectKey];
            _destroyEffectSfxKey = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.DestorySfxKey];
        }

        public override void AddData(string[] arr)
        {
            Debug.LogWarning("사용하지 않음");
        }

        public override bool HasDataArray()
        {
            Debug.LogWarning("사용하지 않음");
            return false;
        }


        private static Sprite _instanceSprite;

        private BulletData(string name)
        {
            var obj = new GameObject();
            obj.name = name;
            var sprite = obj.AddComponent<SpriteRenderer>();


            if (_instanceSprite == null)
            {
                Texture2D texture = new Texture2D(100, 100);

                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                }
                _instanceSprite = Sprite.Create(texture, new Rect(0, 0, 100, 100), Vector2.one * 0.5f);
            }

            sprite.sprite = _instanceSprite;
        }
#endif

    }
}