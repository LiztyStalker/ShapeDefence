namespace SDefence.UI
{
    using Packet;
    using Asset.Entity;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIClearPopup : MonoBehaviour
    {
        [SerializeField]
        private UIAssetContainer _uiAsset;

        [SerializeField]
        private Button _toLobbyBtn;

        [SerializeField]
        private Button _adbToLobbyBtn;

        [SerializeField]
        private Button _nextLevelBtn;

        private AssetUsableEntity _assetEntity;

        public void Initialize()
        {
            _uiAsset.Initialize();

            _toLobbyBtn.onClick.AddListener(OnToLobbyCommandPacketEvent);
            _adbToLobbyBtn.onClick.AddListener(OnAdsToLobbyCommandPacketEvent);
            _nextLevelBtn.onClick.AddListener(OnNextLevelCommandPacketEvent);
        }

        public void CleanUp()
        {
            _uiAsset.CleanUp();

            _toLobbyBtn.onClick.RemoveListener(OnToLobbyCommandPacketEvent);
            _adbToLobbyBtn.onClick.RemoveListener(OnAdsToLobbyCommandPacketEvent);
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

        public void SetData(AssetUsableEntity assetEntity)
        {
            _uiAsset.Show();
            _uiAsset.SetData(assetEntity);
            _assetEntity = assetEntity;
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnToLobbyCommandPacketEvent()
        {
            //ToLobbyCommandPacket
            var pk = new ToLobbyCommandPacket();
            pk.AssetEntity = _assetEntity;
            pk.IsClear = true;
            _cmdEvent?.Invoke(pk);
            Hide();
        }
        private void OnAdsToLobbyCommandPacketEvent()
        {
            //AdbToLobbyCommandPacket
            var pk = new AdsToLobbyCommandPacket();
            pk.AssetEntity = _assetEntity;
            _cmdEvent?.Invoke(pk);
        }

        private void OnNextLevelCommandPacketEvent()
        {
            //NextLevelCommandPacket
            var pk = new NextLevelCommandPacket();
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