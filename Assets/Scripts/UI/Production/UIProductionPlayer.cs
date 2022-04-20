namespace SDefence.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIProductionPlayer : MonoBehaviour
    {
        [SerializeField]
        private Button _applyBtn;

        public void Initialize()
        {

        }

        public void CleanUp()
        {

        }

        public void Show(System.Action endCallback)
        {
            gameObject.SetActive(true);
            _applyBtn.onClick.AddListener(() =>
            {
                endCallback?.Invoke();
                Hide();
            });
        }


        public void Hide()
        {
            _applyBtn.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        }


    }
}