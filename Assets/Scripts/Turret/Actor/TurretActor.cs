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
    using SDefence.Durable.Usable;

    public class TurretActor : MonoBehaviour, IDamagable, IPoolElement, IActor, IAttackable
    {
        private GameObject _graphicObject;

        private TurretEntity _entity;
        private DurableBattleEntity _durableEntity;

        private float _nowActionTime;

        private bool _isBroken = false;
        private float _nowRepairTime = 0f;

        private bool _isInvincible = true;
        public bool IsDamagable => !_isInvincible || !_isBroken;

        public string Key => _entity.Key;

        public Vector2 AttackPos => transform.position;

        public Vector2 NowPosition => transform.position;

        public IAttackUsableData AttackUsableData => _entity.GetAttackUsableData();

        public void Activate() 
        {
            _nowActionTime = 0f;
            _nowRepairTime = 0f;
            _isBroken = false;
            gameObject.SetActive(true);
        }
        public void Inactivate() 
        {
            gameObject.SetActive(false);
        }
        public void SetInvincible(bool isInvincible)
        {
            _isInvincible = isInvincible;
        }

        public void Reset()
        {
            _nowActionTime = 0f;
            _nowRepairTime = 0f;
            _isBroken = false;
        }

        public void CleanUp() 
        {
            _entity = null;
            _durableEntity.CleanUp();
            _durableEntity = null;
        }

        public void SetEntity(TurretEntity entity)
        {
            _entity = entity;
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
            OnActorBattlePacketEvent();
        }


        public string GetDurableValue<T>() where T : IDurableUsableData => _durableEntity.GetValue<T>();
        public float GetDurableRate<T>() where T : IDurableUsableData => _durableEntity.GetRate<T>();

        /// <summary>
        /// 다음 웨이브
        /// </summary>
        public void NextWave()
        {
            //현재 실드만
            _durableEntity.Add(_entity.GetRecoveryUsableData<ShieldRecoveryUsableData>());
            OnActorBattlePacketEvent();
            //battleEvent
        }

        public void RunProcess(float deltaTime)
        {
            if (gameObject.activeSelf)
            {
                if (_isBroken)
                {
                    _nowActionTime = 0f;
                    //수리 완료
                    _nowRepairTime += deltaTime;
                    //TurretRepair
                    if (_nowRepairTime >= _entity.RepairTime)
                    {
                        //TurretRepaired
                        _nowRepairTime = 0f;
                        _isBroken = false;
                    }
                }
                else
                {
                    _nowActionTime = Mathf.Clamp(_nowActionTime + deltaTime, 0f, _entity.GetAttackUsableData().Delay);
                    //battleUpdate
                    if (_nowActionTime >= _entity.GetAttackUsableData().Delay)
                    {
                        OnAttackEvent(_entity.BulletKey, this);
                        _nowActionTime -= _entity.GetAttackUsableData().Delay;
                    }
                }
            }
        }

        /// <summary>
        /// 피격받음
        /// </summary>
        /// <param name="data"></param>
        public void SetDamage(IAttackUsableData data)
        {
            //무적이 아니고 충돌체가 있으면
            if (!_isInvincible && GetComponent<Collider2D>() != null)
            {
                if (!_isBroken)
                {
                    OnHitBattlePacketEvent(_durableEntity.GetRate<ShieldDurableUsableData>() > 0);

                    _durableEntity.Subject(data);
                    OnActorBattlePacketEvent();


                    if (_durableEntity.IsZero<HealthDurableUsableData>())
                    {
                        //파괴됨 상태
                        _isBroken = true;
                        OnDestroyBattlePacketEvent();
                    }
                }
            }
        }



        public static TurretActor Create(int orbitIndex = 0)
        {
            var actor = new GameObject();
            actor.name = "Actor@Turret";
            if (orbitIndex != 0)
            {
                var col = actor.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
            }
            return actor.AddComponent<TurretActor>();
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

        private void OnHitBattlePacketEvent(bool isShieldHit)
        {
            var packet = new HitBattlePacket();
            packet.NowPosition = transform.position;
            packet.IsShieldHit = isShieldHit;
            _battleEvent?.Invoke(packet);
        }

        private void OnDestroyBattlePacketEvent()
        {
            var packet = new DestroyBattlePacket();
            packet.Actor = this;
            _battleEvent?.Invoke(packet);
        }

        private System.Action<string, IAttackable> _attackEvent;
        public void AddOnAttackListener(System.Action<string, IAttackable> act) => _attackEvent += act;
        public void RemoveOnAttackListener(System.Action<string, IAttackable> act) => _attackEvent -= act;
        private void OnAttackEvent(string bulletKey, IAttackable attackable)
        {
            _attackEvent?.Invoke(bulletKey, attackable);
        }

        private System.Action<IPoolElement> _retrieveEvent;
        public void AddOnRetrieveListener(System.Action<IPoolElement> act) => _retrieveEvent += act;
        public void RemoveOnRetrieveListener(System.Action<IPoolElement> act) => _retrieveEvent -= act;

        private void OnRetrieveEvent()
        {
            _retrieveEvent?.Invoke(this);
        }
        #endregion
    }
}