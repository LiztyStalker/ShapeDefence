namespace Utility.Bullet
{
    using System.Collections.Generic;
    using UnityEngine;
    using PoolSystem;
    using Data;

    public class BulletManager
    {
        private static PoolSystem<BulletActor> _pool;

        //flyweightPattern
        private static List<BulletActor> _list;

        private static GameObject _gameObject;
        private static GameObject gameObject
        {
            get
            {
                if (_gameObject == null)
                {
                    _gameObject = new GameObject();
                    _gameObject.transform.position = Vector3.zero;
                    _gameObject.name = "Manager@Bullet";
                    Object.DontDestroyOnLoad(_gameObject);
                }
                return _gameObject;
            }
        }

        private static BulletManager _current;

        public static BulletManager Current
        {
            get
            {
                if(_current == null)
                {
                    _current = new BulletManager();                    
                }
                return _current;
            }
        }

        private BulletManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pool = new PoolSystem<BulletActor>();
            _pool.Initialize(Create);
            _list = new List<BulletActor>();
        }

        public void CleanUp()
        {
            _pool.CleanUp();
            _list.Clear();
        }

        /// <summary>
        /// 탄환을 생성합니다
        /// Edit : 실행안함
        /// Play : BulletActor 생성
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startPos"></param>
        /// <param name="arrivePos"></param>
        /// <param name="arrivedCallback"></param>
        /// <returns></returns>
        public BulletActor Activate(BulletData data, float scale, Vector2 startPos, Vector2 arrivePos, System.Action<BulletActor> arrivedCallback = null, System.Action<BulletActor> inactiveCallback = null)
        {
            if (Application.isPlaying)
            {
                if (data == null)
                {
                    Debug.LogError("BulletData를 지정하세요");
                    return null;
                }

                var actor = _pool.GiveElement();
                actor.SetData(data);
                actor.SetPosition(startPos, arrivePos);
                actor.SetOnArrivedListener(arrivedCallback);
                actor.SetOnInactiveListener(actor =>
                {
                    inactiveCallback?.Invoke(actor);
                    RetrieveActor(actor);
                });
                actor.SetScale(scale);
                actor.Activate();
                _list.Add(actor);
                return actor;
            }
            return null;
        }

        /// <summary>
        /// 탄환을 종료합니다
        /// </summary>
        /// <param name="data"></param>
        public void Inactivate(BulletActor actor)
        {
            if (_list.Contains(actor))
            {
                actor.Inactivate();
            }
            else
            {
                Debug.Assert(false, "BulletData를 찾을 수 없습니다");
            }
        }

        private void RetrieveActor(BulletActor actor)
        {
            _list.Remove(actor);
            _pool.RetrieveElement(actor);
        }


        private BulletActor Create()
        {
            var obj = new GameObject();
            var actor = obj.AddComponent<BulletActor>();
            actor.transform.SetParent(gameObject.transform);
            return actor;
        }



    }
}