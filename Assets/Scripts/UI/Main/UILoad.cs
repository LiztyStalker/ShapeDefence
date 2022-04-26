namespace SDefence.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UILoad : MonoBehaviour
    {

        [SerializeField]
        private Text _loadLabel;

        [SerializeField]
        private Text _loadValueLabel;

        [SerializeField]
        private Slider _slider;

        public static UILoad Create()
        {
            var obj = new GameObject();
            obj.name = "UI@Load";
            return obj.AddComponent<UILoad>();
        }

        public void Initialize()
        {
            _loadLabel.text = "·ÎµùÁß";
            _loadValueLabel.text = "0";

            Hide();

        }

        public void CleanUp()
        {

        }

        public void ShowLoad(float progress)
        {
            _loadValueLabel.text = progress.ToString();
            _slider.value = progress;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}