namespace SDefence.UI
{
    using Packet;
    using Asset;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITechPopupButton : Button
    {
        [SerializeField]
        private Text _text;

        private string _type;
        private string _key;

        private int _orbitIndex;
        private int _index;

        private UIAssetContainer _uiAssetContainer;

        private IAssetUsableData _assetData;

        protected override void Awake()
        {
            onClick.AddListener(OnUpTechEvent);
            _uiAssetContainer = GetComponentInChildren<UIAssetContainer>(true);
            _uiAssetContainer.Initialize();
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnUpTechEvent);
            _uiAssetContainer.CleanUp();
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetIndex(int orbitIndex, int index)
        {
            _orbitIndex = orbitIndex;
            _index = index;
        }

        public void SetData(string type, string key, IAssetUsableData assetData, bool interactable)
        {
            _text.text = key; //Translate
            _type = type;
            _key = key;

            _assetData = assetData;
            _uiAssetContainer.SetData(assetData);

            this.interactable = interactable;
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnUpTechEvent()
        {
            var pk = new UpTechCommandPacket();
            pk.TypeCmdKey = (TYPE_COMMAND_KEY)System.Enum.Parse(typeof(TYPE_COMMAND_KEY), _type);
            pk.ParentIndex = _orbitIndex;
            pk.Index = _index;
            pk.Key = _key;
            pk.AssetUsableData = _assetData;
            _cmdEvent?.Invoke(pk);
        }

        #endregion
    }
}