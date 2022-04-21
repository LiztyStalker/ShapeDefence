namespace SDefence.UI
{
    using Data;
    using SDefence.Packet;
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

        public void SetData(LevelWaveData levelWaveData)
        {
            _levelText.text = $"Level {levelWaveData.GetLevel()}";
        }

        public void OnBattlePacketEvent(IBattlePacket packet)
        {

        }
    }
}