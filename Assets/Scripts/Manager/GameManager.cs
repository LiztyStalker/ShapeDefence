namespace SDefence.Manager
{
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        private GameSystem _system;
        private BattleManager _battle;

        private void Awake()
        {
            _battle = BattleManager.Create();
            _system = GameSystem.Create();
            _system.Initialize();

            _system.AddOnRefreshEntityPacketListener(_battle.OnEntityPacketEvent);
        }

        private void Start()
        {
            _system.RefreshAll();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _battle.RunProcess(deltaTime);
        }

        private void OnDestroy()
        {
            _system.RemoveOnRefreshEntityPacketListener(_battle.OnEntityPacketEvent);

            _battle = null;
            _system = null;
        }


#if UNITY_EDITOR
        public void OnCommandPacketEvent(Packet.ICommandPacket packet)
        {
            _system.OnCommandPacketEvent(packet);
            _battle.OnCommandPacketEvent(packet);
        }
#endif

    }
}