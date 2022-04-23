namespace Utility.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UICreditsSheet : MonoBehaviour
    {
        [SerializeField]
        private Button _exitBtn;


        public void Initialize()
        {
            _exitBtn.onClick.AddListener(Hide);
        }

        public void CleanUp()
        {
            _exitBtn.onClick.RemoveListener(Hide);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}