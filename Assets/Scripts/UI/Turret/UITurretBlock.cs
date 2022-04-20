namespace SDefence.UI
{
    using SDefence.Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITurretBlock : MonoBehaviour
    {

        [SerializeField]
        private Image _icon;

        //Text

        [SerializeField]
        private Button _disassembleBtn;

        [SerializeField]
        private UIAssetButton _upgradeBtn;

        [SerializeField]
        private UIAssetButton _techBtn;

        private void Awake()
        {
            _disassembleBtn.onClick.AddListener(OnDisassembleCommandPacketEvent);
            _upgradeBtn.onClick.AddListener(OnUpgradeCommandPacketEvent);
            _techBtn.onClick.AddListener(OnTechCommandPacketEvent);
        }

        private void OnDestroy()
        {
            _disassembleBtn.onClick.RemoveListener(OnDisassembleCommandPacketEvent);
            _upgradeBtn.onClick.RemoveListener(OnUpgradeCommandPacketEvent);
            _techBtn.onClick.RemoveListener(OnTechCommandPacketEvent);
        }

        public void OnEntityPacketEvent(IEntityPacket pk)
        {
            //Text
            //Activate Inactivate            
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;

        private void OnUpgradeCommandPacketEvent()
        {
            //UpgradeCommandPacket
            var pk = new UpgradeCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        private void OnTechCommandPacketEvent()
        {
            //OpenTechCommandPacket
            var pk = new OpenTechCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        private void OnDisassembleCommandPacketEvent()
        {
            var pk = new OpenDisassembleCommandPacket();
            _cmdEvent?.Invoke(pk);
        }
        #endregion
    }
}