namespace SDefence.Actor
{
    using PoolSystem;
    using Turret.Entity;
    using UnityEngine;
    using Durable.Entity;
    using Recovery.Usable;
    using Attack;
    using Packet;
    using Durable;
    using Durable.Usable;
    using Movement;
    using Attack.Usable;
    using Enemy;
    using Asset;

    public class EnemyActor : MonoBehaviour, IDamagable, IPoolElement, IActor, IAttackable, IMoveable
    {
        private GameObject _graphicObject;

        private EnemyEntity _entity;
        private DurableBattleEntity _durableEntity;

        private float _nowActionTime;

        private bool _isBroken = false;

        private IMovementActionUsableData _movementAction;


        public string Key => _entity.Key;
        public Vector2 AttackPos => transform.position;
        public Vector2 NowPosition => transform.position;
        public bool IsDamagable => !_isBroken;
        public TYPE_ENEMY_STYLE TypeEnemyStyle => _entity.TypeEnemyStyle;
        public IAttackUsableData AttackUsableData => _entity.GetAttackUsableData();

        public IAssetUsableData RewardAssetUsableData => _entity.GetRewardAssetUsableData();

        public void SetPosition(Vector2 pos) => transform.position = pos;

        public void Activate() 
        {
            _nowActionTime = 0f;
            _isBroken = false;
            gameObject.SetActive(true);
        }
        public void Inactivate() 
        {
            gameObject.SetActive(false);
        }

        public void ForceRetrieve()
        {
            OnRetrieveEvent();
        }

        public void CleanUp() 
        {
            _entity = null;
            _durableEntity.CleanUp();
            _durableEntity = null;
        }

        public void SetEntity(EnemyEntity entity)
        {
            _entity = entity;
            _movementAction = _entity.GetMovementActionUsableData();
        }

        public void SetGraphicObject(GameObject graphicObject)
        {
            if (_graphicObject != null) DestroyImmediate(_graphicObject);

            _graphicObject = Instantiate(graphicObject);
            _graphicObject.name = "GraphicObject";
            _graphicObject.transform.SetParent(transform);
            _graphicObject.transform.localPosition = Vector3.zero;
            _graphicObject.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 전투 돌입
        /// </summary>
        public void SetDurableBattleEntity()
        {
            _durableEntity = _entity.GetDurableBattleEntity();
        }

        public string GetDurableValue<T>() where T : IDurableUsableData => _durableEntity.GetValue<T>();
        public float GetDurableRate<T>() where T : IDurableUsableData => _durableEntity.GetRate<T>();


        public void RunProcess(float deltaTime, Vector2 target)
        {
            if (gameObject.activeSelf)
            {
                if (!_isBroken)
                {
                    if (_entity.IsAttack)
                    {
                        _nowActionTime += deltaTime;
                        //battleUpdate
                        if (_nowActionTime >= AttackUsableData.Delay)
                        {
                            OnAttackEvent(_entity.BulletKey, this);
                            _nowActionTime -= AttackUsableData.Delay;
                        }
                    }

                    _movementAction.RunProcess(this, _entity.GetMovementUsableData(), deltaTime, target);
                }
            }
        }

        /// <summary>
        /// 피격받음
        /// </summary>
        /// <param name="data"></param>
        public void SetDamage(IAttackUsableData data)
        {
            if (!_isBroken)
            {
                _durableEntity.Subject(data);
                OnActorBattlePacketEvent();
                //Debug.Log(_durableEntity.GetValue<HealthDurableUsableData>() + " " + data.CreateUniversalUsableData().Value);

                if (_durableEntity.IsZero<HealthDurableUsableData>())
                {
                    //파괴됨 상태
                    _isBroken = true;
                    //battleBrokenEvent
                    OnRetrieveEvent();
                    OnDestroyEvent(true);
                }
            }
            //battleEvent
        }


        public void OnTriggerEnter2D(Collider2D col)
        {
            var damagable = col.GetComponent<IDamagable>();
            if(damagable != null)
            {
                if (damagable is HQActor || damagable is TurretActor)
                {
                    if (damagable.IsDamagable)
                    {
                        var usable = new AttackUsableData();
                        usable.SetData(_durableEntity.GetDurableUsableData<HealthDurableUsableData>().CreateUniversalUsableData());
                        damagable.SetDamage(usable);

                    }
                    _isBroken = true;
                    OnRetrieveEvent();
                    OnDestroyEvent(false);
                }
            }
        }

        public static EnemyActor Create()
        {
            var actor = new GameObject();
            actor.name = "Actor@Turret";
            var col = actor.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            var rigid = actor.AddComponent<Rigidbody2D>();
            rigid.gravityScale = 0f;
            return actor.AddComponent<EnemyActor>();
        }

        #region ##### Listener #####

        private System.Action<IBattlePacket> _battleEvent;
        public void AddOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent += act;
        public void RemoveOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent -= act;
        private void OnActorBattlePacketEvent()
        {
            var packet = new ActorBattlePacket();
            packet.Actor = this;
            _battleEvent?.Invoke(packet);
        }             

        private System.Action<string, IAttackable> _attackEvent;
        public void AddOnAttackListener(System.Action<string, IAttackable> act) => _attackEvent += act;
        public void RemoveOnAttackListener(System.Action<string, IAttackable> act) => _attackEvent -= act;
        private void OnAttackEvent(string bulletKey, IAttackable attackable) => _attackEvent?.Invoke(bulletKey, attackable);

        private System.Action<EnemyActor> _retrieveEvent;
        public void AddOnRetrieveListener(System.Action<EnemyActor> act) => _retrieveEvent += act;
        public void RemoveOnRetrieveListener(System.Action<EnemyActor> act) => _retrieveEvent -= act;
        private void OnRetrieveEvent() => _retrieveEvent?.Invoke(this);


        private System.Action<EnemyActor, bool> _destroyEvent;
        public void SetOnDestroyListener(System.Action<EnemyActor, bool> act) => _destroyEvent = act;
        private void OnDestroyEvent(bool isReward) => _destroyEvent?.Invoke(this, isReward);


        #endregion

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (_entity.IsAttack)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, AttackUsableData.Range);
            }
        }
#endif

    }
}