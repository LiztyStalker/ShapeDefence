namespace SDefence.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIStart : MonoBehaviour
    {

        [SerializeField]
        private Button _startButton;

        private System.Action _startEvent;


        public static UIStart Create()
        {
            var obj = new GameObject();
            obj.name = "UI@Start";
            return obj.AddComponent<UIStart>();
        }

        public void Initialize()
        {
            _startButton.onClick.AddListener(OnStartEvent);
            Hide();
        }

        public void CleanUp()
        {
            _startButton.onClick.RemoveListener(OnStartEvent);
        }

        public void ShowStart(System.Action startCallback)
        {
            gameObject.SetActive(true);
            _startEvent = startCallback;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnStartEvent()
        {
            _startEvent?.Invoke();
        }
    }
}