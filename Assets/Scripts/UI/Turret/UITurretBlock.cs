namespace SDefence.UI
{
    using SDefence.Packet;
    using SDefence.Turret.Entity;
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

        private int _orbitIndex;
        private int _index;

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

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            if(packet is TurretEntityPacket)
            {
                var pk = (TurretEntityPacket)packet;
                var entity = pk.Entity;

                //Index
                _orbitIndex = pk.OrbitIndex;
                _index = pk.Index;
                
                
                //Text





                _upgradeBtn.SetActive(!entity.IsMaxUpgrade());
                _techBtn.SetActive(entity.IsMaxUpgrade());

                //_disassembleBtn.interactable = //Tech 0 = false
                _upgradeBtn.interactable = pk.IsActiveUpgrade;
                _techBtn.interactable = pk.IsActiveUpTech;


            }
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;

        private void OnUpgradeCommandPacketEvent()
        {
            //UpgradeCommandPacket
            var pk = new UpgradeCommandPacket();
            pk.typeCmdKey = TYPE_COMMAND_KEY.Turret;
            pk.ParentIndex = _orbitIndex;
            pk.Index = _index;
            _cmdEvent?.Invoke(pk);
        }

        private void OnTechCommandPacketEvent()
        {
            //OpenTechCommandPacket
            var pk = new OpenTechCommandPacket();
            pk.typeCmdKey = TYPE_COMMAND_KEY.Turret;
            pk.ParentIndex = _orbitIndex;
            pk.Index = _index;
            _cmdEvent?.Invoke(pk);
        }

        private void OnDisassembleCommandPacketEvent()
        {
            var pk = new OpenDisassembleCommandPacket();
            pk.TypeCmdKey = TYPE_COMMAND_KEY.Turret;
            pk.ParentIndex = _orbitIndex;
            pk.Index = _index;
            _cmdEvent?.Invoke(pk);
        }
        #endregion
    }
}