namespace SDefence.UI
{
    using UnityEngine;


    public class UIHQ : MonoBehaviour, ICategory
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