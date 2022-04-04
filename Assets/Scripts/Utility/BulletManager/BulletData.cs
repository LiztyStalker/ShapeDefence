namespace Utility.Bullet.Data
{
    using UnityEngine;
    using Storage;
    using Effect.Data;
    using ScriptableObjectData;

    [CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/BulletData")]
    public class BulletData : ScriptableObjectData
    {
        public enum TYPE_BULLET_ACTION { Move, Curve, Drop, Direct }

        [SerializeField]
        private GameObject _bulletPrefab;

        [SerializeField]
        private string _bulletPrefabKey;

        [SerializeField]
        private TYPE_BULLET_ACTION _typeBulletAction;

        [SerializeField]
        private EffectData _arriveEffectData;

        [SerializeField]
        private string _arriveEffectDataKey;

        [SerializeField]
        private float _movementSpeed = 1f;

        [SerializeField]
        private float _height = 0f;

        [SerializeField]
        private bool _isRotate = false;

#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public static BulletData CreateTest()
        {
            return new BulletData("Data@Bullet");
        }

        public void SetData(TYPE_BULLET_ACTION typeBulletAction, float movementSpeed, bool isRotate)
        {
            _typeBulletAction = typeBulletAction;
            _movementSpeed = movementSpeed;
            _isRotate = isRotate;
        }

        public override void SetData(string[] arr)
        {
            Key = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.Key];

            name = $"{typeof(BulletData).Name}_{Key}";

            _typeBulletAction = (TYPE_BULLET_ACTION)System.Enum.Parse(typeof(TYPE_BULLET_ACTION), arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.TypeBulletAction]);
            _movementSpeed = float.Parse(arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.MoveSpeed]);
            _height = float.Parse(arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.Height]);
            _isRotate = bool.Parse(arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.IsRotate]);

            if (arr.Length > (int)BulletDataGenerator.TYPE_SHEET_COLUMNS.BulletSpriteKey)
            {
                _bulletPrefabKey = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.BulletSpriteKey];
                _arriveEffectDataKey = arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.BulletHitKey];
            }
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

        //public override string[] GetData()
        //{
        //    string[] arr = new string[System.Enum.GetValues(typeof(BulletDataGenerator.TYPE_SHEET_COLUMNS)).Length];

        //    arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.TypeBulletAction] = _typeBulletAction.ToString();
        //    arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.MoveSpeed] = _movementSpeed.ToString();
        //    arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.Height] = _height.ToString();
        //    arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.IsRotate] = _isRotate.ToString();
        //    arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.BulletSpriteKey] = _bulletPrefabKey;
        //    arr[(int)BulletDataGenerator.TYPE_SHEET_COLUMNS.BulletHitKey] = _arriveEffectDataKey;

        //    return arr;
        //}

        //public override string[][] GetDataArray()
        //{
        //    Debug.LogWarning("사용하지 않음");
        //    return null;
        //}

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
            _bulletPrefab = obj;
            _bulletPrefab.gameObject.SetActive(false);
        }
#endif


        #region ##### Getter Setter #####
        public GameObject prefab
        {
            get
            {
                if (_bulletPrefab == null && !string.IsNullOrEmpty(_bulletPrefabKey))
                    _bulletPrefab = DataStorage.Instance.GetDataOrNull<GameObject>(_bulletPrefabKey, "Bullet", null);
                return _bulletPrefab;
            }
        }
        public EffectData ArriveEffectData
        {
            get
            {
                if (_arriveEffectData == null && !string.IsNullOrEmpty(_arriveEffectDataKey))
                    _arriveEffectData = DataStorage.Instance.GetDataOrNull<EffectData>(_arriveEffectDataKey);
                return _arriveEffectData;
            }
        }
        public bool IsRotate => _isRotate;
        public float MovementSpeed => _movementSpeed;
        public TYPE_BULLET_ACTION TypeBulletAction => _typeBulletAction;

        #endregion

    }
}