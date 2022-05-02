namespace SDefence.UI
{
    using Packet;
    using Asset.Entity;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIDefeatPopup : MonoBehaviour
    {
        [SerializeField]
        private UIAssetContainer _uiAsset;

        [SerializeField]
        private Button _toLobbyBtn;

        [SerializeField]
        private Button _retryBtn;

        private AssetUsableEntity _assetEntity;
        public void Initialize()
        {
            _uiAsset.Initialize();
            _toLobbyBtn.onClick.AddListener(OnToLobbyCommandPacketEvent);
            _retryBtn.onClick.AddListener(OnRetryCommandPacketEvent);
        }

        public void CleanUp()
        {
            _uiAsset.CleanUp();
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

        public void SetData(AssetUsableEntity assetEntity)
        {
            _uiAsset.Show();
            _uiAsset.SetData(assetEntity);
            _assetEntity = assetEntity;
            //IAssetUsableData
        }

        #region ##### Listener #####
        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnToLobbyCommandPacketEvent()
        {
            var pk = new ToLobbyCommandPacket();
            pk.AssetEntity = _assetEntity;
            pk.IsClear = false;
            _cmdEvent?.Invoke(pk);
            Hide();
        }
        private void OnRetryCommandPacketEvent()
        {
            var pk = new RetryCommandPacket();
            pk.AssetEntity = _assetEntity;
            _cmdEvent?.Invoke(pk);
            Hide();
        }


        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}