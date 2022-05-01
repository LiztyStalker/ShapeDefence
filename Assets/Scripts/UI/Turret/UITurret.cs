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

        private int _OrbitIndex;

        public void Initialize()
        {
            _list = new List<UITurretTab>();

            var btn = DataStorage.Instance.GetDataOrNull<GameObject>("UI@TabBtn");
            _tabBtn = btn.GetComponent<UITurretTab>();

            _helpBtn.onClick.AddListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.AddListener(Hide);

            _mainTurret.SetOnCommandPacketListener(OnCommandPacketEvent);
            _orbitTurret.SetOnCommandPacketListener(OnCommandPacketEvent);

            _orbitTurret.Initialize();
            _orbitTurret.Hide();

        }
        public void CleanUp()
        {
            _mainTurret.SetOnCommandPacketListener(null);
            _orbitTurret.SetOnCommandPacketListener(null);

            _helpBtn.onClick.RemoveListener(OnHelpCommandPacketEvent);
            _exitBtn.onClick.RemoveListener(Hide);

            _orbitTurret.CleanUp();
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

            switch (packet)
            {
                case TurretOrbitEntityPacket pk:
                    while (_list.Count < pk.OrbitCount)
                    {
                        var btn = Instantiate(_tabBtn);
                        btn.transform.SetParent(_tabFrame);
                        btn.transform.localScale = Vector2.one;
                        if(_list.Count == 0)
                            btn.SetText($"ÁÖÆ÷Å¾");
                        else
                            btn.SetText($"±Ëµµ{_list.Count}");
                        btn.SetIndex(_list.Count);
                        btn.SetOnCommandPacketListener(OnCommandPacketEvent);
                        _list.Add(btn);
                    }
                    break;
                case TurretEntityPacket pk:
                    if(pk.OrbitIndex == 0)
                    {
                        _orbitTurret.Hide();
                        _mainTurret.Show();
                        _mainTurret.OnEntityPacketEvent(packet);
                    }
                    _OrbitIndex = pk.OrbitIndex;

                    break;
                case TurretArrayEntityPacket pk:
                    _mainTurret.Hide();
                    _orbitTurret.Show();
                    _orbitTurret.OnEntityPacketEvent(packet);

                    _OrbitIndex = pk.OrbitIndex;

                    break;
                case TurretExpandEntityPacket pk:
                    _orbitTurret.OnEntityPacketEvent(packet);
                    break;
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

        private void OnRefreshCommandPacketEvent()
        {
            var pk = new RefreshCommandPacket();
            pk.TypeCmdKey = TYPE_COMMAND_KEY.Turret;
            pk.ParentIndex = _OrbitIndex;
            _cmdEvent?.Invoke(pk);
        }


        private System.Action _closedEvent;
        public void SetOnClosedListener(System.Action act) => _closedEvent = act;
        private void OnClosedEvent() => _closedEvent?.Invoke();

        #endregion
    }
}