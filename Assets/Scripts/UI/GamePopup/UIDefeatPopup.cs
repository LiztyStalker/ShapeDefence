namespace SDefence.UI
{
    using Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIDefeatPopup : MonoBehaviour
    {
        [SerializeField]
        private UIAsset _uiAsset;

        [SerializeField]
        private Button _toLobbyBtn;

        [SerializeField]
        private Button _retryBtn;

        public void Initialize()
        {
            _toLobbyBtn.onClick.AddListener(OnToLobbyCommandPacketEvent);
            _retryBtn.onClick.AddListener(OnRetryCommandPacketEvent);
        }

        public void CleanUp()
        {
            _toLobbyBtn.onClick.RemoveListener(OnToLobbyCommandPacketEvent);
            _retryBtn.onClick.AddListener(OnRetryCommandPacketEvent);
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
        }
        private void OnRetryCommandPacketEvent()
        {
            //AdbToLobbyCommandPacket
            var pk = new RetryCommandPacket();
            _cmdEvent?.Invoke(pk);
        }


        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}