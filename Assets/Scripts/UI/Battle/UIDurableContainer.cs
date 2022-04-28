namespace SDefence.UI
{
    using Actor;
    using Enemy;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine;
    using PoolSystem;

    public class UIDurableContainer : MonoBehaviour
    {

        [SerializeField]
        private UIDurableBlock _bossBlock;

        [SerializeField]
        private UIDurableBlock _hqBlock;

        private UIDurableBlock _durableBlock;

        [SerializeField]
        private RectTransform _frameContainer;


        private PoolSystem<UIDurableBlock> _pool;

        private Dictionary<IActor, UIDurableBlock> _dic;

        public void Initialize()
        {
            _pool = new PoolSystem<UIDurableBlock>();
            _pool.Initialize(CreateBlock);

            _dic = new Dictionary<IActor, UIDurableBlock>();

            var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@DurableBlock");
            _durableBlock = obj.GetComponent<UIDurableBlock>();
        }

        public void CleanUp()
        {
            _dic.Clear();
        }

        public void Show(TYPE_ENEMY_STYLE typeEnemyStyle)
        {
            if (IsBoss(typeEnemyStyle))
            {
                _bossBlock.Show();
            }
        }

        public void HideBoss()
        {
            _bossBlock.Hide();
        }

        public void SetData(IActor actor)
        {
            Debug.Log("Actor" + actor);
            switch (actor)
            {
                case HQActor hActor:
                    _hqBlock.SetData(hActor);
                    break;
                case EnemyActor eActor:
                    if (IsBoss(eActor.TypeEnemyStyle))
                    {
                        _bossBlock.SetData(actor);
                    }
                    break;
                //    else
                //    {
                //        if (!_dic.ContainsKey(eActor))
                //        {
                //            var block = _pool.GiveElement();
                //            _dic.Add(eActor, block);
                //        }
                //        _dic[eActor].SetData(eActor);
                //        _dic[eActor].Show();
                //    }
                //    break;
                //case TurretActor tActor:
                //    if (!_dic.ContainsKey(tActor))
                //    {
                //        var block = _pool.GiveElement();
                //        _dic.Add(tActor, block);
                //    }
                //    _dic[tActor].SetData(tActor);
                //    _dic[tActor].Show();
                //    break;
            }
        }

        private bool IsBoss(TYPE_ENEMY_STYLE typeEnemyStyle)
        {
            switch (typeEnemyStyle)
            {
                case TYPE_ENEMY_STYLE.SpecialBoss:
                case TYPE_ENEMY_STYLE.ThemeBoss:
                case TYPE_ENEMY_STYLE.NormalBoss:
                case TYPE_ENEMY_STYLE.MiddleBoss:
                    return true;
            }
            return false;
        }

        private UIDurableBlock CreateBlock()
        {
            var block = Instantiate(_durableBlock);
            block.name = "UI@DurableBlock";
            block.transform.SetParent(_frameContainer);
            block.transform.localScale = Vector3.one;
            block.SetOnRetrieveListener(OnRetrieveEvent);
            return block;
        }

        private void OnRetrieveEvent(IActor actor)
        {
            var block = _dic[actor];
            _dic.Remove(actor);
            _pool.RetrieveElement(block);
            block.Hide();
        }
    }
}