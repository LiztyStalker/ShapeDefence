namespace SDefence.UI
{
    using Packet;
    using Durable.Usable;
    using Recovery.Usable;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITurretBlock : MonoBehaviour
    {

        [SerializeField]
        private Image _icon;

        //Text
        [SerializeField]
        private Text _text;

        [SerializeField]
        private Text[] _texts;

        [SerializeField]
        private Button _disassembleBtn;

        [SerializeField]
        private UIAssetButton _upgradeBtn;

        [SerializeField]
        private Button _techBtn;

        private int _orbitIndex;
        private int _index;

        public void Initialize()
        {
            _upgradeBtn.Initialize();

            _disassembleBtn.onClick.AddListener(OnDisassembleCommandPacketEvent);
            _upgradeBtn.onClick.AddListener(OnUpgradeCommandPacketEvent);
            _techBtn.onClick.AddListener(OnTechCommandPacketEvent);
        }

        public void CleanUp()
        {
            _upgradeBtn.CleanUp();

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


                if (_text != null)
                {
                    //Text
                    string str = "";
                    str += entity.Name + " " + entity.TechLevel + ((entity.IsMaxUpgradeAndTech()) ? "Max\n" : "\n");
                    str += entity.UpgradeValue + "\n";
                    str += "?????? " + entity.GetDurableUsableData<HealthDurableUsableData>() + "\n";
                    str += "?????? " + entity.GetDurableUsableData<ArmorDurableUsableData>() + "\n";
                    str += "????????" + entity.RepairTime + "s\n";
                    str += "???? " + entity.GetDurableUsableData<ShieldDurableUsableData>() + "\n";
                    str += "???????? " + entity.GetRecoveryUsableData<ShieldRecoveryUsableData>() + "w\n";
                    str += "???????? " + entity.GetDurableUsableData<LimitDamageShieldDurableUsableData>() + "\n";
                    str += "?????? " + entity.GetAttackUsableData().CreateUniversalUsableData().Value + "\n";
                    str += "???????? " + entity.GetAttackUsableData().Delay + "s\n";
                    str += "??????" + entity.GetAttackUsableData().Range;

                    _text.text = str;
                }
                else
                {
                    string str = "";
                    str += entity.Name + " " + entity.TechLevel + ((entity.IsMaxUpgradeAndTech()) ? "Max\n" : "\n");
                    str += entity.UpgradeValue;
                    _texts[0].text = str;

                    str = "?????? " + entity.GetDurableUsableData<HealthDurableUsableData>() + "\n";
                    str += "?????? " + entity.GetDurableUsableData<ArmorDurableUsableData>() + "\n";
                    str += "????????" + entity.RepairTime + "s";
                    _texts[1].text = str;

                    str = "???? " + entity.GetDurableUsableData<ShieldDurableUsableData>() + "\n";
                    str += "???????? " + entity.GetRecoveryUsableData<ShieldRecoveryUsableData>() + "w\n";
                    str += "???????? " + entity.GetDurableUsableData<LimitDamageShieldDurableUsableData>();
                    _texts[2].text = str;

                    str = "?????? " + entity.GetAttackUsableData().CreateUniversalUsableData().Value + "\n";
                    str += "???????? " + entity.GetAttackUsableData().Delay + "s\n";
                    str += "?????? " + entity.GetAttackUsableData().Range;
                    _texts[3].text = str;
                }


                if (!entity.IsMaxUpgradeAndTech())
                {
                    _upgradeBtn.SetActive(!entity.IsMaxUpgrade());
                    _techBtn.gameObject.SetActive(entity.IsMaxUpgrade());
                }
                else
                {
                    //???? ??????????
                    _upgradeBtn.SetActive(false);
                    _techBtn.gameObject.SetActive(false);
                }

                _disassembleBtn.interactable = entity.TechLevel > 1;
                _upgradeBtn.SetAsset(entity.GetUpgradeAssetUsableData());
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
            pk.TypeCmdKey = TYPE_COMMAND_KEY.Turret;
            pk.ParentIndex = _orbitIndex;
            pk.Index = _index;
            _cmdEvent?.Invoke(pk);
        }

        private void OnTechCommandPacketEvent()
        {
            //OpenTechCommandPacket
            var pk = new OpenTechCommandPacket();
            pk.TypeCmdKey = TYPE_COMMAND_KEY.Turret;
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