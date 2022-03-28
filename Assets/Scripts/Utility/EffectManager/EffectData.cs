namespace Utility.Effect.Data
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EffectData", menuName = "ScriptableObjects/EffectData")]
    public class EffectData : ScriptableObject
    {
        [SerializeField]
        private GameObject _effectPrefab;
        public GameObject effectPrefab => _effectPrefab;


#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        public static EffectData CreateTest()
        {
            return new EffectData();
        }

        private static ParticleSystem _instanceParticleSystem;

        private EffectData()
        {
            var obj = new GameObject();
            obj.name = "Data@Effect";
            var particle = obj.AddComponent<ParticleSystem>();

            if (_instanceParticleSystem == null)
            {
                var module = particle.main;
                module.loop = false;
            }
            _effectPrefab = obj;
            _effectPrefab.gameObject.SetActive(false);
        }
#endif

    }
}