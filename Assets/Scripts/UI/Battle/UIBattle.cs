namespace SDefence.UI
{
    using UnityEngine;

    public class UIBattle : MonoBehaviour
    {
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
    }
}