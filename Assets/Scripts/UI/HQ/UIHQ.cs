namespace SDefence.UI
{
    using Packet;
    using Durable.Usable;
    using Recovery.Usable;
    using Storage;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIHQ : MonoBehaviour, ICategory
    {

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private Text _text;
        //UIGridText

        [SerializeField]
        private UIAssetContainer _uiAsset;

        [SerializeField]
        private Button _helpBtn;

        [SerializeField]
        private Button _exitBtn;

        [SerializeField]
        private UIAssetButton _upgradeBtn;

        [SerializeField]
        private Button _techBtn;

       

        public void Initialize()
        {
            _upgradeBtn.onClick.AddListener(OnUpgradeCommandPacketEvent);
            _techBtn.onClick.AddListener(OnTechCommandPacketEvent);

            _helpBtn.onClick.AddListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.AddListener(Hide);

            _upgradeBtn.Initialize();

            _uiAsset.Initialize();
        }
        public void CleanUp()
        {
            _upgradeBtn.onClick.RemoveListener(OnUpgradeCommandPacketEvent);
            _techBtn.onClick.RemoveListener(OnTechCommandPacketEvent);

            _helpBtn.onClick.RemoveListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.RemoveListener(Hide);

            _upgradeBtn.CleanUp();

            _uiAsset.CleanUp();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            OnRefreshCommandPacketEvent();
        }        

        public void Hide()
        {
            gameObject.SetActive(false);
            OnClosedEvent();
        }

        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            if (gameObject.activeSelf)
            {
                switch (packet)
                {
                    case HQEntityPacket pk:
                        RefreshUI(pk);
                        break;
                }
                _uiAsset.OnEntityPacketEvent(packet);
            }
        }

        private void RefreshUI(HQEntityPacket packet)
        {
            var entity = packet.Entity;

            _icon.sprite = DataStorage.Instance.GetDataOrNull<Sprite>(entity.IconKey, "Icon");

            //UIGridText
            string str = "";
            str += entity.Name + "\n";
            str += entity.TechLevel + ((entity.IsMaxUpgradeAndTech()) ? "Max\n" : "\n");
            str += entity.UpgradeValue + "\n";
            str += "내구도 " + entity.GetDurableUsableData<HealthDurableUsableData>() + "\n";
            str += "방어력 " + entity.GetDurableUsableData<ArmorDurableUsableData>() + "\n";
            str += "실드 " + entity.GetDurableUsableData<ShieldDurableUsableData>() + "\n";
            str += "실드회복량 " + entity.GetRecoveryUsableData<ShieldRecoveryUsableData>() + "\n";
            str += "실드최대피격량 " + entity.GetDurableUsableData<LimitDamageShieldDurableUsableData>() + "\n";
            str += "궤도수 " + entity.TurretCount + "\n";

            _text.text = str;

            //Upgrade - Asset
            //Tech - Asset
            if (!entity.IsMaxUpgradeAndTech())
            {                
                _upgradeBtn.SetActive(!entity.IsMaxUpgrade());
                _techBtn.gameObject.SetActive(entity.IsMaxUpgrade());
            }
            else
            {
                //Max Upgrade
                _upgradeBtn.SetActive(false);
                _techBtn.gameObject.SetActive(false);
            }

            _upgradeBtn.SetAsset(entity.GetUpgradeData());

            _upgradeBtn.interactable = packet.IsActiveUpgrade;
            _techBtn.interactable = packet.IsActiveUpTech;

        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnHelpCommandPacketEvent()
        {
            //HelpCommandPacket
            var pk = new HelpCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        private void OnUpgradeCommandPacketEvent()
        {
            //UpgradeCommandPacket
            var pk = new UpgradeCommandPacket();
            pk.TypeCmdKey = TYPE_COMMAND_KEY.HQ;
            _cmdEvent?.Invoke(pk);
        }

        private void OnTechCommandPacketEvent()
        {
            //OpenTechCommandPacket
            var pk = new OpenTechCommandPacket();
            pk.TypeCmdKey = TYPE_COMMAND_KEY.HQ;
            _cmdEvent?.Invoke(pk);
        }

        private void OnRefreshCommandPacketEvent()
        {
            var pk = new RefreshCommandPacket();
            pk.TypeCmdKey = TYPE_COMMAND_KEY.HQ;
            _cmdEvent?.Invoke(pk);
        }

        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}