namespace SDefence.UI
{
    using SDefence.Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIRewardOfflinePopup : MonoBehaviour
    {
        [SerializeField]
        private UIAssetContainer _uiAsset;

        [SerializeField]
        private Text _timeText;

        [SerializeField]
        private Button _rewardBtn;

        [SerializeField]
        private Button _adbRewardBtn;

        public void Initialize()
        {
            _rewardBtn.onClick.AddListener(OnRewardCommandPacketEvent);
            _adbRewardBtn.onClick.AddListener(OnAdbRewardCommandPacketEvent);
        }

        public void CleanUp()
        {
            _rewardBtn.onClick.RemoveListener(OnRewardCommandPacketEvent);
            _adbRewardBtn.onClick.AddListener(OnAdbRewardCommandPacketEvent);
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
        private void OnRewardCommandPacketEvent()
        {
            //ToLobbyCommandPacket
            var pk = new RewardOfflineCommandPacket();
            pk.IsAdb = false;
            _cmdEvent?.Invoke(pk);
        }
        private void OnAdbRewardCommandPacketEvent()
        {
            //AdbToLobbyCommandPacket
            var pk = new RewardOfflineCommandPacket();
            pk.IsAdb = true;
            _cmdEvent?.Invoke(pk);
        }



        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}