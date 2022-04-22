namespace SDefence.UI
{
    using Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITurretTab : Button
    {
        [SerializeField]
        private Text _text;

        private int _index;

        public void SetIndex(int index) => _index = index;
        public void SetText(string text) => _text.text = text;

        protected override void Awake()
        {
            onClick.AddListener(OnCommandPacketEvent);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnCommandPacketEvent);
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnCommandPacketEvent()
        {
            var pk = new TabCommandPacket();
            pk.Index = _index;
            _cmdEvent?.Invoke(pk);
        }
        #endregion
    }
}