namespace Utility.Bullet
{
    using System.Collections.Generic;
    using UnityEngine;
    using PoolSystem;
    using Data;
    using SDefence.Attack;
    using SDefence.Attack.Usable;
    using SDefence.Actor;
    using Storage;

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
                    if(Application.isPlaying)
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
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startPos"></param>
        /// <param name="arrivePos"></param>
        /// <param name="arrivedCallback"></param>
        /// <returns></returns>
        public BulletActor Activate(
            IAttackable attackable, 
            BulletData data, 
            float scale, 
            Vector2 startPos, 
            Vector2 arrivePos, 
            System.Action<BulletActor, IAttackable, IDamagable, AttackActionUsableData> attackCallback, 
            System.Action<BulletActor> retrieveCallback)
        {
            if (data == null)
            {
                Debug.LogError("BulletData를 지정하세요");
                return null;
            }

            var prefab = DataStorage.Instance.GetDataOrNull<GameObject>(data.GraphicObjectKey);

            var actor = _pool.GiveElement();
            actor.SetData(data);
            actor.SetData(attackable);
            actor.SetPosition(startPos, arrivePos);
            if(prefab != null) actor.SetGraphicObject(prefab);
            actor.SetOnAttackListener(attackCallback);
            actor.SetOnArrivedListener(actor =>
            {
                retrieveCallback?.Invoke(actor);
                RetrieveActor(actor);
            });
            actor.SetScale(scale);
            actor.Activate();
            _list.Add(actor);
            return actor;
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

        public void RunProcess(float deltaTime)
        {
            for(int i = 0; i < _list.Count; i++)
            {
                _list[i].RunProcess(deltaTime);
            }
        }

        private void RetrieveActor(BulletActor actor)
        {
            actor.Inactivate();
            _list.Remove(actor);
            _pool.RetrieveElement(actor);
        }


        private BulletActor Create()
        {
            var actor = BulletActor.Create();
            actor.transform.SetParent(gameObject.transform);
            return actor;
        }
    }
}