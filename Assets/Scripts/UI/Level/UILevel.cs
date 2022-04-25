namespace SDefence.UI
{
    using Packet;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILevel : MonoBehaviour, IBattlePacketUser
    {
        [SerializeField]
        private Text _levelText;

        public void Initialize()
        {

        }

        public void CleanUp()
        {

        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnBattlePacketEvent(IBattlePacket packet)
        {
            if(packet is LevelWaveBattlePacket)
            {
                var pk = (LevelWaveBattlePacket)packet;
                _levelText.text = $"Level {pk.data.GetLevel()}";
            }

        }
    }
}