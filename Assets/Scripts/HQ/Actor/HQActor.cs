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

    public class HQActor : MonoBehaviour, IDamagable, IPoolElement, IActor
    {
        private GameObject _graphicObject;

        private HQEntity _entity;
        private DurableBattleEntity _durableEntity;

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

        /// <summary>
        /// ���� ����
        /// </summary>
        public void SetDurableBattleEntity()
        {
            _durableEntity = _entity.GetDurableBattleEntity();
            OnActorPacketEvent();
        }

        /// <summary>
        /// ���� �̵���
        /// </summary>
        public void UnsetDurableBattleEntity()
        {
            _durableEntity = null;
        }

        public string GetDurableValue<T>() where T : IDurableUsableData => _durableEntity.GetValue<T>();

        /// <summary>
        /// ���� ���̺�
        /// </summary>
        public void NextWave()
        {
            //���� �ǵ常
            _durableEntity.Add(_entity.GetRecoveryUsableData<ShieldRecoveryUsableData>());
            OnActorPacketEvent();
        }

        /// <summary>
        /// �ǰݹ���
        /// </summary>
        /// <param name="data"></param>
        public void SetDamage(IAttackUsableData data)
        {
            _durableEntity.Subject(data);
            OnActorPacketEvent();
        }


#if UNITY_EDITOR
        public static HQActor Create()
        {
            var actor = new GameObject();            
            actor.AddComponent<CircleCollider2D>();
            return actor.AddComponent<HQActor>();
        }
#endif

        #region ##### Listener #####

        private System.Action<IBattlePacket> _actorEvent;
        public void AddOnActorPacketListener(System.Action<IBattlePacket> act) => _actorEvent += act;
        public void RemoveOnActorPacketListener(System.Action<IBattlePacket> act) => _actorEvent -= act;
        private void OnActorPacketEvent()
        {
            var packet = new HQBattlePacket();
            packet.SetData(this);
            _actorEvent?.Invoke(packet);
        }
        #endregion
    }
}