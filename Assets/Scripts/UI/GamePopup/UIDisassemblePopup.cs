namespace SDefence.UI
{
    using Packet;
    using Entity;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIDisassemblePopup : MonoBehaviour
    {

        [SerializeField]
        private Text _text;

        [SerializeField]
        private UIAssetContainer _uiAsset;

        [SerializeField]
        private Button _exitBtn;

        [SerializeField]
        private Button _closeBtn;

        [SerializeField]
        private Button _applyBtn;

        public void Initialize()
        {
            _uiAsset.Initialize();
            _closeBtn.onClick.AddListener(OnCloseEvent);
            _exitBtn.onClick.AddListener(OnCloseEvent);
            _applyBtn.onClick.AddListener(OnDisassembleEvent);
        }

        public void CleanUp()
        {
            _uiAsset.CleanUp();
            _closeBtn.onClick.AddListener(OnCloseEvent);
            _exitBtn.onClick.AddListener(OnCloseEvent);
            _applyBtn.onClick.AddListener(OnDisassembleEvent);
        }

        public void Show(IEntity entity)
        {
            //Entity
            //Return IAssetUsableData
            //StarAssetUsableData

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
        private void OnCloseEvent()
        {
            Hide();
        }
        private void OnDisassembleEvent()
        {
            //DisassembleCommandPacket - 50%
            //PerfectDisassembleCommandPacket - 100%
            var pk = new DisassembleCommandPacket();
            _cmdEvent?.Invoke(pk);
            OnCloseEvent();
        }



        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}