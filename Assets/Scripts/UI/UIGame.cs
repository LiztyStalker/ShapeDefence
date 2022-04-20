namespace SDefence.UI
{
    using UnityEngine;
    using Storage;
    using Packet;
    using System.Collections.Generic;
    using Utility.UI;


    #region ##### Category #####
    public interface ICategory 
    {
        public void Initialize();
        public void CleanUp();
        public void Show();
        public void Hide();
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act);
    }

    #endregion


    public class UIGame : MonoBehaviour
    {
        private UILobby _uiLobby;
        private UIBattle _uiBattle;
        private UIGamePopup _uiGamePopup;
        private UIProduction _uiProduction;
        private UILevel _uiLevel;
        private UIAsset _uiAsset;
        private UIHelp _uiHelp;
        private UIButtons _uiButtons;
        private UICommon _uiCommon;

        private Dictionary<string, ICategory> _dic;



        public void Initialize()
        {
            _uiCommon = UICommon.Current;
            _uiLobby = GetCanvas(_uiLobby, "UI@Lobby");
            _uiBattle = GetCanvas(_uiBattle, "UI@Battle");
            _uiGamePopup = GetCanvas(_uiGamePopup, "UI@GamePopup");
            _uiProduction = GetCanvas(_uiProduction, "UI@Production");
            _uiLevel = GetCanvas(_uiLevel, "UI@Level");
            _uiAsset = GetCanvas(_uiAsset, "UI@Asset");
            _uiHelp = GetCanvas(_uiHelp, "UI@Help");
            _uiButtons = GetCanvas(_uiButtons, "UI@Buttons");

#if UNITY_EDITOR
            Debug.Assert(_uiLobby != null, "_uiLobby 를 찾을 수 없음");
            Debug.Assert(_uiBattle != null, "_uiBattle 를 찾을 수 없음");
            Debug.Assert(_uiGamePopup != null, "_uiGamePopup 를 찾을 수 없음");
            Debug.Assert(_uiProduction != null, "_uiProduction 를 찾을 수 없음");
            Debug.Assert(_uiLevel != null, "_uiLevel 를 찾을 수 없음");
            Debug.Assert(_uiAsset != null, "_uiAsset 를 찾을 수 없음");
            Debug.Assert(_uiHelp != null, "_uiHelp 를 찾을 수 없음");
            Debug.Assert(_uiButtons != null, "_uiButtons 를 찾을 수 없음");

#endif
            _dic = new Dictionary<string, ICategory>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var category = child.GetComponent<ICategory>();
                if (category != null)
                {
                    _dic.Add(category.GetType().Name, category);
                }
            }

            foreach(var value in _dic.Values)
            {
                value.Initialize();
                value.Hide();
                value.SetOnCommandPacketListener(OnCommandPacketEvent);
            }

            _uiLobby.Initialize();
            _uiLobby.AddOnCommandPacketListener(OnCommandPacketEvent);
            _uiLobby.Show();

            _uiBattle.Initialize();
            _uiBattle.Hide();

            _uiGamePopup.Initialize();

            _uiProduction.Initialize();
            _uiProduction.Hide();

            _uiLevel.Initialize();
            _uiLevel.Show();

            _uiAsset.Initialize();
            _uiAsset.Show();

            _uiHelp.Initialize();
            _uiHelp.Hide();

            _uiButtons.Initialize();
            _uiButtons.Show();
        }

        public void CleanUp()
        {
            foreach (var value in _dic.Values)
            {
                value.SetOnCommandPacketListener(null);
                value.CleanUp();
            }
            _dic.Clear();

            _uiLobby.RemoveOnCommandPacketListener(OnCommandPacketEvent);
            _uiLobby.CleanUp();

            _uiBattle.CleanUp();

            _uiGamePopup.CleanUp();

            _uiProduction.CleanUp();

            _uiLevel.CleanUp();

            _uiAsset.CleanUp();

            _uiHelp.CleanUp();

            _uiButtons.CleanUp();

            _uiCommon.CleanUp();
        }

        private T GetCanvas<T>(T ui, string name) where T : MonoBehaviour
        {
            if (ui == null)
            {
                var child = FindObjectOfType<T>(true);
                if (child == null)
                {
                    var obj = DataStorage.Instance.GetDataOrNull<GameObject>(name);
                    ui = obj.GetComponent<T>();
                }
                else
                {
                    ui = child.GetComponent<T>();
                }
            }
            return ui;
        }


        public void OnEntityPacketEvent(IEntityPacket pk)
        {
            //모두 뿌리기?
            //IEntityPacketUser
        }

        public void OnBattlePacketEvent(IBattlePacket pk)
        {
            //모두 뿌리기?
            //IBattlePacketUser
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void AddOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent += act;
        public void RemoveOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent -= act;
        private void OnCommandPacketEvent(ICommandPacket packet)
        {

            switch (packet)
            {
                case SettingsCommandPacket pk:
                    //UICommon Settings
                    UICommon.Current.ShowSettings();
                    break;
                case OpenDisassembleCommandPacket pk:
                    //GamePopup Disassemble
                    break;
                case OpenTechCommandPacket pk:
                    //GamePopup Tech
                    break;
                case DisassembleCommandPacket pk:
                    //Production Disassemble
                    _uiProduction.ShowDisassembleProduction(null);
                    break;
                case UpTechCommandPacket pk:
                    //Production UpTech
                    _uiProduction.ShowTechProduction(null);
                    break;
                case HelpCommandPacket pk:
                    //UIHelp Show
                    _uiHelp.Show();
                    break;

                case PlayBattleCommandPacket pk:
                    //Play Battle
                    _uiLobby.Hide();
                    _uiBattle.Show();
                    break;

                case CategoryCommandPacket pk:
                    //Menu Category
                    if (_dic.ContainsKey(pk.Category))
                    {
                        _dic[pk.Category].Show();
                    }
                    break;
            }
            _cmdEvent?.Invoke(packet);
        }

        #endregion
    }
}