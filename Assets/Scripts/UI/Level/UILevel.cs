namespace SDefence.UI
{
    using Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class UILevel : MonoBehaviour
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

    }
}