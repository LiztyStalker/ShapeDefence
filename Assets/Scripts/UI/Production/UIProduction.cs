namespace SDefence.UI
{
    using UnityEngine;
    public class UIProduction : MonoBehaviour
    {

        [SerializeField]
        private UIProductionPlayer _disassembleProduction;

        [SerializeField]
        private UIProductionPlayer _techProduction;

        public void Initialize()
        {
            _disassembleProduction.Hide();
            _techProduction.Hide();
        }

        public void CleanUp()
        {

        }

        public void ShowDisassembleProduction(System.Action endCallback)
        {
            gameObject.SetActive(true);
            _disassembleProduction.Show(() => {
                endCallback?.Invoke();
                Hide();
                });
        }

        public void ShowTechProduction(System.Action endCallback)
        {
            gameObject.SetActive(true);
            _techProduction.Show(() => {
                endCallback?.Invoke();
                Hide();
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}