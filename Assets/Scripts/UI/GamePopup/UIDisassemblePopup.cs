namespace SDefence.UI
{
    using Packet;
    using UnityEngine;
    using UnityEngine.UI;
    using Asset.Entity;

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

        private AssetUsableEntity _assetEntity;
        private int _orbitIndex;
        private int _index;

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

        public void Show(int orbitIndex, int index, AssetUsableEntity entity)
        {
            //Return IAssetUsableData
            //StarAssetUsableData
            _orbitIndex = orbitIndex;
            _index = index;
            _assetEntity = entity;
            _uiAsset.SetData(entity);
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
            pk.TypeCmdKey = TYPE_COMMAND_KEY.Turret;
            pk.AssetEntity = _assetEntity;
            pk.ParentIndex = _orbitIndex;
            pk.Index = _index;
            _cmdEvent?.Invoke(pk);
            OnCloseEvent();
        }



        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}