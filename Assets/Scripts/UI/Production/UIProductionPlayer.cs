namespace SDefence.UI
{
    using Entity;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIProductionPlayer : MonoBehaviour
    {
        [SerializeField]
        private Text _text;

        [SerializeField]
        private Button _applyBtn;

        [SerializeField]
        private Button _skipBtn;

        private float _nowTime;

        public void Show(IEntity entity, System.Action endCallback)
        {
            _applyBtn.gameObject.SetActive(false);
            _skipBtn.gameObject.SetActive(true);

            _nowTime = 1f;

            gameObject.SetActive(true);

            //���� Entity or Data �ʿ�
            //���� Entity or Data �ʿ�

            _skipBtn.onClick.AddListener(() =>
            {
                _nowTime = 0f;
            });

            _applyBtn.onClick.AddListener(() =>
            {
                endCallback?.Invoke();
                Hide();
            });
        }


        public void Hide()
        {
            _skipBtn.onClick.RemoveAllListeners();
            _applyBtn.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if(_nowTime < 0f)
            {
                _text.text = "���� �Ϸ�";
                _applyBtn.gameObject.SetActive(true);
                _skipBtn.gameObject.SetActive(false);
            }
            else
            {
                _nowTime -= Time.deltaTime;
            }

            _text.text =  "���� " + _nowTime.ToString();
        }

    }
}