namespace SDefence.UI
{
    using UnityEngine;
    using Storage;
    using Packet;
    using System.Collections.Generic;


    #region ##### Category #####
    public interface ICategory 
    {
        public void Initialize();
        public void CleanUp();
        public void Show();
        public void Hide();
    }

    #endregion


    public class UIGame : MonoBehaviour
    {
        private UILobby _uiLobby;
        private UIBattle _uiBattle;

        private Dictionary<string, ICategory> _dic = new Dictionary<string, ICategory>();



        public void Initialize()
        {

            _uiLobby = GetCanvas(_uiLobby, "UI@Lobby");
            _uiBattle = GetCanvas(_uiBattle, "UI@Battle");

#if UNITY_EDITOR
            Debug.Assert(_uiLobby != null, "_uiLobby 를 찾을 수 없음");
            Debug.Assert(_uiBattle != null, "_uiBattle 를 찾을 수 없음");
#endif

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
            }

            _uiLobby.Initialize();
            _uiLobby.AddOnCommandPacketListener(OnCommandPacketEvent);
            _uiLobby.Show();

            _uiBattle.Initialize();
            _uiBattle.Hide();
        }

        public void CleanUp()
        {
            foreach (var value in _dic.Values)
            {
                value.CleanUp();
            }
            _dic.Clear();

            _uiLobby.RemoveOnCommandPacketListener(OnCommandPacketEvent);
            _uiLobby.CleanUp();

            _uiBattle.CleanUp();
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

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void AddOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent += act;
        public void RemoveOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent -= act;
        private void OnCommandPacketEvent(ICommandPacket packet)
        {
            switch (packet)
            {
                case PlayBattleCommandPacket pk:
                    _uiLobby.Hide();
                    _uiBattle.Show();
                    break;
                case CategoryCommandPacket pk:
                    _uiLobby.Hide();
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