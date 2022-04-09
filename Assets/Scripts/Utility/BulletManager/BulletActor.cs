namespace Utility.Bullet
{
    using UnityEngine;
    using PoolSystem;
    using Data;
    using Effect;
    using SDefence.Actor;
    using SDefence.Attack;
    using SDefence.Movement;
    using SDefence.Attack.Usable;

    public class BulletActor : MonoBehaviour, IPoolElement, IMoveable
    {

        private const float ARRIVE_DISTANCE = 0.1f;

        private IAttackable _attackable;

        private GameObject _graphicObject;
        private BulletData _data;

        private Vector2 _startPos;
        private Vector2 _arrivePos;

        private IMovementUsableData _movementData;
        private IMovementActionUsableData _movementActionData;
        private AttackActionUsableData _attackActionData;

        private CircleCollider2D _col;
        private Rigidbody2D _rigid;

        private SpriteRenderer _spriteRenderer { get; set; }
        private ParticleSystem[] _particles { get; set; }

        public Vector2 NowPosition => transform.position;

        public string DestroyEffectDataKey => _data.DestroyEffectDataKey;
        public string DestroyEffectSfxKey => _data.DestroyEffectSfxKey;
        public string ActiveEffectDataKey => _data.ActiveEffectDataKey;
        public string ActiveEffectSfxKey => _data.ActiveEffectSfxKey;

        public static BulletActor Create()
        {
            var obj = new GameObject();
            var col = obj.AddComponent<CircleCollider2D>();
            col.radius = 0.25f;
            col.isTrigger = true;
            var rigid = obj.AddComponent<Rigidbody2D>();
            rigid.gravityScale = 0f;
            var actor = obj.AddComponent<BulletActor>();
            actor.Inactivate();
            return actor;
        }

        public void SetData(BulletData data)
        {
            _data = data;
            _movementData = _data.GetMovementUsableData();
            _movementActionData = _data.GetMovementActionUsableData();
            _attackActionData = _data.GetAttackActionUsableData();
            //도착 후 공격 여부 필요
            _movementActionData.SetOnEndedActionListener(OnArrivedEvent);
            SetName();
        }

        private void SetName()
        {
            if (!Application.isEditor)
            {
                gameObject.name = $"BulletActor_{_data.name}";
            }
        }

        public void SetData(IAttackable attackable)
        {
            _attackable = attackable;
        }

        public void SetGraphicObject(GameObject graphicObject)
        {
            if (_graphicObject != null) DestroyImmediate(_graphicObject);

            _graphicObject = Instantiate(graphicObject);
            _graphicObject.transform.SetParent(transform);
            _graphicObject.transform.localPosition = Vector3.zero;
            _graphicObject.transform.localScale = Vector3.one;
        }

        public void SetPosition(Vector2 startPos, Vector2 arrivePos)
        {
            _startPos = startPos;
            _arrivePos = arrivePos;
        }


        public bool Contains(BulletData data) => _data == data;

        public void Activate()
        {
            transform.position = _startPos;
            gameObject.SetActive(true);
        }

        public void Inactivate()
        {
            gameObject.SetActive(false);
        }

        public void CleanUp()
        {
            _data = null;
            _arrivedEvent = null;
            _startPos = Vector2.zero;
            _arrivePos = Vector2.zero;
        }

        public void SetPosition(Vector2 pos) => transform.position = pos;

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }
        public void RunProcess(float deltaTime)
        {
            _movementActionData.RunProcess(this, _movementData, deltaTime, _arrivePos);
        }

        private void ReadyInactivate()
        {

            for (int i = 0; i < _particles.Length; i++)
            {
                var main = _particles[i].main;
                main.loop = false;
            }

            if (_spriteRenderer != null)
                _spriteRenderer.enabled = false;

            if (_particles == null)
            {
                Inactivate();
            }
            else
            {
                int cnt = 0;
                for (int i = 0; i < _particles.Length; i++)
                {
                    if (!_particles[i].isPlaying)
                    {
                        cnt++;
                    }
                }
                if (cnt == _particles.Length)
                {
                    Inactivate();
                }
            }
        }


        public void OnTriggerEnter2D(Collider2D col)
        {
            var damagable = col.GetComponent<IDamagable>();
            if (damagable != null)
            {
                if (damagable.IsDamagable) 
                {
                    if (_attackable is EnemyActor && damagable is TurretActor || _attackable is TurretActor && damagable is EnemyActor)
                    {
                        //충돌 행동
                        _movementActionData.SetCollision();
                        OnAttackEvent(damagable);

                        //충돌 후 반납 여부 필요
                    }
                }
            }
        }

#if UNITY_EDITOR
        public void SetCollsion()
        {
            _movementActionData.SetCollision();
            OnAttackEvent(null);
        }
#endif



        #region ##### Listener #####

        private System.Action<BulletActor> _arrivedEvent;
        public void SetOnArrivedListener(System.Action<BulletActor> act) => _arrivedEvent = act;
        private void OnArrivedEvent()
        {
            _arrivedEvent?.Invoke(this);
        }


        private System.Action<BulletActor, IAttackable, IDamagable, AttackActionUsableData, System.Action> _attackEvent;
        public void SetOnAttackListener(System.Action<BulletActor, IAttackable, IDamagable, AttackActionUsableData, System.Action> act) => _attackEvent = act;
        private void OnAttackEvent(IDamagable damagable)
        {
            _attackEvent?.Invoke(this, _attackable, damagable, _attackActionData, OnRetrieveEvent);
            OnArrivedEvent();
        }


        private System.Action<BulletActor> _retrieveEvent;
        public void SetOnRetrieveListener(System.Action<BulletActor> act) => _retrieveEvent = act;
        private void OnRetrieveEvent()
        {
            _retrieveEvent?.Invoke(this);
            _attackable = null;
            _movementActionData = null;
            _attackActionData = null;
            _arrivedEvent = null;
            _attackActionData = null;
            _startPos = Vector2.zero;
            _arrivePos = Vector2.zero;
        }

        #endregion


    }
}