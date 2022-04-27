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

        private void ShowDisassembleProduction(IEntity nowEntity, IEntity pastEntity)
        {
            gameObject.SetActive(true);
            _disassembleProduction.Show(nowEntity, pastEntity, Hide);
        }

        private void ShowTechProduction(IEntity nowEntity, IEntity pastEntity)
        {
            gameObject.SetActive(true);
            _techProduction.Show(nowEntity, pastEntity, Hide);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnEntityPacketEvent(IEntityPacket packet)
        {
            switch (packet)
            {
                case DisassembleEntityPacket pk:
                    ShowDisassembleProduction(pk.NowEntity, pk.PastEntity);
                    break;
                case UpTechEntityPacket pk:
                    ShowTechProduction(pk.NowEntity, pk.PastEntity);
                    break;
            }
        }
    }
}