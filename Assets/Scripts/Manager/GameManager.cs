namespace SDefence.Manager
{
    using UnityEngine;
    using UI;
    using Storage;

    public class GameManager : MonoBehaviour
    {
        private GameSystem _system;
        private BattleManager _battle;
        private UIGame _uiGame;

        private void Awake()
        {
            _uiGame = FindObjectOfType<UIGame>(true);
            if(_uiGame == null)
            {
                var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@Game");
                _uiGame = obj.GetComponent<UIGame>();
            }

            _uiGame.Initialize();





            _battle = BattleManager.Create();
            _system = GameSystem.Create();

            _system.Initialize();

            _system.AddOnRefreshEntityPacketListener(_battle.OnEntityPacketEvent);

            _battle.AddOnBattlePacketListener(packet => {
                Debug.Log($"Packet {packet.GetType().Name}");
            });
        }

        private void Start()
        {
            _system.RefreshAll();
            _battle.SetLobby();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            _battle.RunProcess(deltaTime);
        }

        private void OnDestroy()
        {
            _system.RemoveOnRefreshEntityPacketListener(_battle.OnEntityPacketEvent);
           
            _battle.RemoveOnBattlePacketListener(packet => {
                Debug.Log($"Packet {packet.GetType().Name}");
            });

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