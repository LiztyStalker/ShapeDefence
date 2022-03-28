
namespace UtilityManager
{
    using UnityEngine;


    [CreateAssetMenu(fileName = "GameLanguageData", menuName = "ScriptableObjects/GameLanguageData")]

    public class GameLanguageData : ScriptableObject
    {
        [SerializeField]
        private SystemLanguage[] _usableLanguages;

        [SerializeField]
        private SystemLanguage _defaultLanguage;
       
        public SystemLanguage[] UsableLanguages => _usableLanguages;
    }
}