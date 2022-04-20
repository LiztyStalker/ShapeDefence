namespace SDefence.UI
{
    using UnityEngine;

    public class UIBattle : MonoBehaviour
    {
        private UILevelWave _uiLevelWave;
        private UIAlarmBoss _uiAlarmBoss;
        private UIDurableContainer _uiDurableContainer;

        public void Initialize()
        {
            _uiLevelWave = GetComponentInChildren<UILevelWave>(true);
            _uiAlarmBoss = GetComponentInChildren<UIAlarmBoss>(true);
            _uiDurableContainer = GetComponentInChildren<UIDurableContainer>(true);

#if UNITY_EDITOR
            Debug.Assert(_uiLevelWave != null, "_uiLevelWave �� ã�� �� �����ϴ�");
            Debug.Assert(_uiAlarmBoss != null, "_uiAlarmBoss �� ã�� �� �����ϴ�");
            Debug.Assert(_uiDurableContainer != null, "_uiDurableContainer �� ã�� �� �����ϴ�");
#endif

            _uiLevelWave.Initialize();
            _uiAlarmBoss.Initialize();
            _uiDurableContainer.Initialize();
        }

        public void CleanUp()
        {
            _uiLevelWave.CleanUp();
            _uiAlarmBoss.CleanUp();
            _uiDurableContainer.CleanUp();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void SetData()
        {
            //LevelWave
            //AlarmBoss
            //DurableContainer
        }
    }
}