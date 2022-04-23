namespace SDefence.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIHelp : MonoBehaviour
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
            _exitBtn = null;
        }

        public void Show()
        {
            //TYPE_CATEGORY
            //Description
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}