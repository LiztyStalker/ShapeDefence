namespace Utility.Effect
{
    using System.Collections.Generic;
    using UnityEngine;
    using PoolSystem;
    using Data;

    public class EffectManager
    {

        private PoolSystem<EffectActor> _pool;

        private List<EffectActor> _list;

//        private Dictionary<EffectData, List<EffectActor>> _effectDic = new Dictionary<EffectData, List<EffectActor>>();

        private static GameObject _gameObject;

        private static GameObject gameObject
        {
            get
            {
                if (_gameObject == null)
                {
                    _gameObject = new GameObject();
                    _gameObject.transform.position = Vector3.zero;
                    _gameObject.name = "Manager@Effect";
                    Object.DontDestroyOnLoad(_gameObject);
                }
                return _gameObject;
            }
        }


        private static EffectManager _current;

        public static EffectManager Current
        {
            get
            {
                if(_current == null)
                {
                    _current = new EffectManager();
                }
                return _current;
            }
        }

        private EffectManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pool = new PoolSystem<EffectActor>();
            _pool.Initialize(Create);
            _list = new List<EffectActor>();
        }

        public void CleanUp()
        {
            _pool.CleanUp();
            _list.Clear();
            _current = null;
        }

        /// <summary>
        /// EffectData를 GameObject Instance화 합니다
        /// EditMode : 실행되지 않습니다
        /// Play : GameObject가 생성됩니다
        /// </summary>
        /// <param name="effectData"></param>
        /// <param name="position"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public EffectActor Activate(EffectData effectData, Vector3 position, float scale, System.Action<EffectActor> inactiveCallback = null)
        {
            if (effectData != null)
            {
                var actor = _pool.GiveElement();
                actor.SetData(effectData);
                actor.SetOnInactiveListener(actor => {
                    inactiveCallback?.Invoke(actor);
                    RetrieveActor(actor);
                });
                actor.transform.localScale = Vector3.one * scale;
                actor.Activate(position);
                _list.Add(actor);
                return actor;
            }
            return null;
        }


        public EffectActor Activate(GameObject effectObject, Vector3 position, float scale, System.Action<EffectActor> inactiveCallback = null)
        {
            if (effectObject != null)
            {
                var actor = _pool.GiveElement();
                actor.SetData(effectObject);
                actor.SetOnInactiveListener(actor => {
                    inactiveCallback?.Invoke(actor);
                    RetrieveActor(actor);
                });
                actor.transform.localScale = Vector3.one * scale;
                actor.Activate(position);
                _list.Add(actor);
                return actor;
            }
            return null;
        }



        /// <summary>
        /// EffectActor를 종료합니다
        /// </summary>
        /// <param name="effectData"></param>
        public void Inactivate(EffectActor actor)
        {
            if (_list.Contains(actor))
            {
                actor.Inactivate();
            }
        }

        private void RetrieveActor(EffectActor actor)
        {
            _list.Remove(actor);
            _pool.RetrieveElement(actor);
        }

        private EffectActor Create()
        {
            var gameObejct = new GameObject();
            var actor = gameObejct.AddComponent<EffectActor>();
            actor.transform.SetParent(gameObject.transform);
            return actor;
        }



    }
}