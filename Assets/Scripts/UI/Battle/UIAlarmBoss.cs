namespace SDefence.UI
{
    using SDefence.Enemy;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIAlarmBoss : MonoBehaviour
    {
        private const float ALARM_TIME = 2f;

        [SerializeField]
        private Text _text;

        private float _nowTime;

        public void Initialize()
        {
        }

        public void CleanUp()
        {
        }


        public void Show(TYPE_ENEMY_STYLE typeEnemyStyle)
        {
            switch (typeEnemyStyle)
            {
                case TYPE_ENEMY_STYLE.SpecialBoss:
                    ShowSpecialBoss();
                    break;
                case TYPE_ENEMY_STYLE.ThemeBoss:
                    ShowThemeBoss();
                    break;
                case TYPE_ENEMY_STYLE.NormalBoss:
                case TYPE_ENEMY_STYLE.MiddleBoss:
                    ShowBoss();
                    break;
            }
        }

        private void ShowBoss()
        {
            _nowTime = ALARM_TIME;
            _text.text = "!Boss!";
            gameObject.SetActive(true);
        }

        private void ShowThemeBoss()
        {
            _nowTime = ALARM_TIME;
            _text.text = "!!!ThemeBoss!!!";
            gameObject.SetActive(true);
        }

        private void ShowSpecialBoss()
        {
            _nowTime = ALARM_TIME;
            _text.text = "!!SpecialBoss!!";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            _nowTime -= Time.deltaTime;
            if(_nowTime < 0f)
            {
                Hide();
            }
        }
    }
}