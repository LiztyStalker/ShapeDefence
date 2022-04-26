namespace SDefence.Manager
{
    using UnityEngine;
    using UI;
    using Storage;
    using Utility.IO;

    public class GameManager : MonoBehaviour
    {
        private GameSystem _system;
        private BattleManager _battle;
        private UIGame _uiGame;

        private void Awake()
        {
            Application.quitting += OnQuitEvent;

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
                        
            //Event
            _system.AddOnEntityPacketListener(_battle.OnEntityPacketEvent);
            _system.AddOnEntityPacketListener(_uiGame.OnEntityPacketEvent);

            _battle.AddOnBattlePacketListener(_system.OnBattlePacketEvent);
            _battle.AddOnBattlePacketListener(_uiGame.OnBattlePacketEvent);

            _uiGame.AddOnCommandPacketListener(_system.OnCommandPacketEvent);
            _uiGame.AddOnCommandPacketListener(_battle.OnCommandPacketEvent);


            LoadData();
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

        private void LoadData()
        {
            var savable = SavablePackage.Current.GetSavableData();

            if (savable != null)
            {
                //Version
                var systemSavable = savable.GetValue<SavableData>(_system.SavableKey());
                _system.SetSavableData(systemSavable);

                var battleSavable = savable.GetValue<SavableData>(_battle.SavableKey());
                _battle.SetSavableData(battleSavable);
            }
        }

        private void SaveData(System.Action endCallback)
        {
            var data = SavableData.Create();

            data.AddData("Version", Application.version);
            data.AddData(_system.SavableKey(), _system.GetSavableData());
            data.AddData(_battle.SavableKey(), _battle.GetSavableData());

            SavablePackage.Current.SetSavableData(data);
            SavablePackage.Current.Save(result =>
            {
#if UNITY_EDITOR
                Debug.Log($"Save {result}");
#endif
                endCallback?.Invoke();
            });
        }

#if UNITY_EDITOR
        public void OnCommandPacketEvent(Packet.ICommandPacket packet)
        {
            _system.OnCommandPacketEvent(packet);
            _battle.OnCommandPacketEvent(packet);
        }
#endif

        //포커스 저장
        private void OnApplicationFocus(bool focus)
        {
            if(focus) SaveData(null);
        }

        //백그라운드 저장
        private void OnApplicationPause(bool pause)
        {
            if(pause) SaveData(null);
        }

        //게임 종료시 저장 후 종료
        private void OnQuitEvent()
        {
            SaveData(() =>
            {
#if !UNITY_EDITOR
                System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
                Application.quitting -= OnQuitEvent;
                SavablePackage.Dispose();
            });
        }
    }
}