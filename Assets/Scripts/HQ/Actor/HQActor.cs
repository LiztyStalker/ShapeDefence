namespace SDefence.Actor
{
    using PoolSystem;
    using HQ.Entity;
    using UnityEngine;
    using Durable.Entity;
    using SDefence.Recovery.Usable;
    using SDefence.Attack;
    using SDefence.Packet;
    using SDefence.Durable;
    using SDefence.Durable.Usable;

    public class HQActor : MonoBehaviour, IDamagable, IPoolElement, IActor
    {
        private GameObject _graphicObject;

        private HQEntity _entity;
        private DurableBattleEntity _durableEntity;

        public string Key => _entity.Key;

        public Vector2 NowPosition => transform.position;

        public void Activate() 
        {
            gameObject.SetActive(true);
        }
        public void Inactivate() 
        {
            gameObject.SetActive(false);
        }
        public void CleanUp() 
        {
            _entity = null;
            _durableEntity.CleanUp();
            _durableEntity = null;
        }

        public void SetEntity(HQEntity entity)
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
            OnBattlePacketEvent();
        }

        /// <summary>
        /// 전투 미돌입
        /// </summary>
        public void UnsetDurableBattleEntity()
        {
            _durableEntity = null;
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
            OnBattlePacketEvent();
        }

        /// <summary>
        /// 피격받음
        /// </summary>
        /// <param name="data"></param>
        public void SetDamage(IAttackUsableData data)
        {
            _durableEntity.Subject(data);
            OnBattlePacketEvent();
        }

        public bool IsDamagable => true;


#if UNITY_EDITOR
        public static HQActor Create()
        {
            var actor = new GameObject();
            actor.name = "Actor@HQ";
            var col = actor.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            var rigid = actor.AddComponent<Rigidbody2D>();
            rigid.gravityScale = 0f;
            return actor.AddComponent<HQActor>();
        }
#endif

        #region ##### Listener #####

        private System.Action<IBattlePacket> _battleEvent;
        public void AddOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent += act;
        public void RemoveOnBattlePacketListener(System.Action<IBattlePacket> act) => _battleEvent -= act;
        private void OnBattlePacketEvent()
        {
            var packet = new HQBattlePacket();
            packet.SetData(this);
            _battleEvent?.Invoke(packet);
        }
        #endregion
    }
}