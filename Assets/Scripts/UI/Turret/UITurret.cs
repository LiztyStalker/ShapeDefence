namespace SDefence.UI
{
    using Packet;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UITurret : MonoBehaviour, ICategory
    {
        [SerializeField]
        private UIAssetContainer _uiAsset;

        [SerializeField]
        private Button _helpBtn;

        [SerializeField]
        private Button _exitBtn;

        [SerializeField]
        private RectTransform _tabFrame;

        [SerializeField]
        private UITurretBlock _mainTurret;

        [SerializeField]
        private UITurretSheet _orbitTurret;

        private UITurretTab _tabBtn;

        private List<UITurretTab> _list;

        public void Initialize()
        {
            _list = new List<UITurretTab>();

            var btn = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TabBtn");
            _tabBtn = btn.GetComponent<UITurretTab>();

            _helpBtn.onClick.AddListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.AddListener(Hide);

            _mainTurret.SetOnCommandPacketListener(OnCommandPacketEvent);
            _orbitTurret.SetOnCommandPacketListener(OnCommandPacketEvent);

            _uiAsset.Initialize();

            _orbitTurret.Hide();

        }
        public void CleanUp()
        {
            _mainTurret.SetOnCommandPacketListener(null);
            _orbitTurret.SetOnCommandPacketListener(null);

            _helpBtn.onClick.RemoveListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.RemoveListener(Hide);

            _uiAsset.CleanUp();
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
            if (packet is TurretEntityPacket)
            {
                //UIAsset
                //tab SetText SetIndex
                //MainTurret
                //OrbitTurret
            }
            _uiAsset.OnEntityPacketEvent(packet);
        }

        #region ##### Listener #####

        private System.Action<ICommandPacket> _cmdEvent;
        public void SetOnCommandPacketListener(System.Action<ICommandPacket> act) => _cmdEvent = act;
        private void OnCommandPacketEvent(ICommandPacket pk) => _cmdEvent?.Invoke(pk);
        private void OnHelpCommandPacketEvent()
        {
            //HelpCommandPacket
            var pk = new HelpCommandPacket();
            _cmdEvent?.Invoke(pk);
        }

        #endregion
    }
}