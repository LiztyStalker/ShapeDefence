namespace SDefence.UI
{
    using Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIClearPopup : MonoBehaviour
    {
        [SerializeField]
        private UIAsset _uiAsset;

        [SerializeField]
        private Button _toLobbyBtn;

        [SerializeField]
        private Button _adbToLobbyBtn;

        [SerializeField]
        private Button _nextLevelBtn;

        public void Initialize()
        {
            _toLobbyBtn.onClick.AddListener(OnToLobbyCommandPacketEvent);
            _adbToLobbyBtn.onClick.AddListener(OnAdbToLobbyCommandPacketEvent);
            _nextLevelBtn.onClick.AddListener(OnNextLevelCommandPacketEvent);
        }

        public void CleanUp()
        {
            _toLobbyBtn.onClick.RemoveListener(OnToLobbyCommandPacketEvent);
            _adbToLobbyBtn.onClick.RemoveListener(OnAdbToLobbyCommandPacketEvent);
            _nextLevelBtn.onClick.RemoveListener(OnNextLevelCommandPacketEvent);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OnClosedEvent();
        }

        public void SetData()
        {
            //AssetData
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnToLobbyCommandPacketEvent()
        {
            //ToLobbyCommandPacket
            var pk = new ToLobbyCommandPacket();
            _cmdEvent?.Invoke(pk);
            Hide();
        }
        private void OnAdbToLobbyCommandPacketEvent()
        {
            //AdbToLobbyCommandPacket
            var pk = new AdbToLobbyCommandPacket();
            _cmdEvent?.Invoke(pk);
            Hide();
        }

        private void OnNextLevelCommandPacketEvent()
        {
            //NextLevelCommandPacket
            var pk = new NextLevelCommandPacket();
            _cmdEvent?.Invoke(pk);
            Hide();
        }




        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();


        #endregion
    }
}