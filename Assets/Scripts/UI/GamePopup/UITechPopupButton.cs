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
        private IAssetUsableData _usableData;

        protected override void Awake()
        {
            onClick.AddListener(OnUpTechEvent);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnUpTechEvent);
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetData(string type, string key, IAssetUsableData usableData)
        {
            _text.text = key; //Translate
            _type = type;
            _key = key;
            _usableData = usableData; //Icon Asset
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnUpTechEvent()
        {
            var pk = new UpTechCommandPacket();
            pk.TypeCmdKey = (TYPE_COMMAND_KEY)System.Enum.Parse(typeof(TYPE_COMMAND_KEY), _type);
            pk.Key = _key;
            pk.AssetUsableData = _usableData;
            _cmdEvent?.Invoke(pk);
        }

        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            //IAssetUsableData
        }
        #endregion
    }
}