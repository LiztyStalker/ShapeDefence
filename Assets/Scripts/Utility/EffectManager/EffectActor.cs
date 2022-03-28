namespace Utility.Effect
{
    using UnityEngine;
    using PoolSystem;
    using Data;

    //public enum TYPE_EFFECT_LIFESPAN { }

    public class EffectActor : MonoBehaviour, IPoolElement
    {
        private GameObject _prefab { get; set; }
        private EffectData _data { get; set; }

        private ParticleSystem[] _particles { get; set; }


        private void SetName()
        {
            gameObject.name = $"EffectActor_{_data.name}";
        }

        public void SetData(EffectData effectData)
        {
            _data = effectData;
            SetName();
        }


        public bool Contains(EffectData effectData) => _data == effectData;

        public void Activate(Vector2 position)
        {
            gameObject.SetActive(true);

            if (_prefab == null)
            {
                _prefab = Instantiate(_data.effectPrefab);
                _prefab.transform.SetParent(transform);
                _prefab.transform.localPosition = Vector3.zero;
                _prefab.transform.localScale = Vector3.one * 0.1f;
                _prefab.gameObject.SetActive(true);
            }

            transform.position = position;

            _particles = _prefab.GetComponentsInChildren<ParticleSystem>();
            if (_particles != null)
            {
                for (int i = 0; i < _particles.Length; i++)
                {
                    var psRenderer = _particles[i].GetComponent<ParticleSystemRenderer>();
                    if (psRenderer != null)
                    {
                        psRenderer.sortingLayerName = "FrontEffect";
                    }
                }
            }
        }

        private void Update()
        {
            if (_particles != null)
            {
                int cnt = 0;
                for (int i = 0; i < _particles.Length; i++)
                {
                    if (!_particles[i].isPlaying)
                    {
                        cnt++;
                    }
                }

                if (cnt == _particles.Length)
                {
                    Inactivate();
                }
            }
        }

        public void Inactivate()
        {
            gameObject.SetActive(false);
            _inactiveEvent?.Invoke(this);
            _inactiveEvent = null;
        }

        public void CleanUp()
        {
            DestroyImmediate(_prefab);
            _data = null;
            _particles = null;
            _inactiveEvent = null;
        }



        #region ##### Listener #####

        private System.Action<EffectActor> _inactiveEvent { get; set; }
        public void SetOnInactiveListener(System.Action<EffectActor> callback) => _inactiveEvent = callback;

        #endregion



    }
}