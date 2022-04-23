namespace SDefence.UI
{
    using Entity;
    using Packet;
    using UnityEngine;
    public class UIProduction : MonoBehaviour, IEntityPacketUser
    {

        [SerializeField]
        private UIProductionPlayer _disassembleProduction;

        [SerializeField]
        private UIProductionPlayer _techProduction;

        public void Initialize()
        {
            _disassembleProduction.Hide();
            _techProduction.Hide();
        }

        public void CleanUp()
        {

        }

        public void ShowDisassembleProduction(System.Action endCallback)
        {
            gameObject.SetActive(true);
            _disassembleProduction.Show(null, () => {
                endCallback?.Invoke();
                Hide();
                });
        }

        private void ShowTechProduction(IEntity entity)
        {
            gameObject.SetActive(true);
            _techProduction.Show(entity, Hide);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            switch (packet)
            {
                case UpTechEntityPacket pk:
                    ShowTechProduction(pk.Entity);
                    break;
            }
        }
    }
}