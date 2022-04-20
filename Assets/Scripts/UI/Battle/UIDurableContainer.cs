namespace SDefence.UI
{
    using Storage;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIDurableContainer : MonoBehaviour
    {

        [SerializeField]
        private UIDurableBlock _bossBlock;

        [SerializeField]
        private UIDurableBlock _hqBlock;

        private UIDurableBlock _durableBlock;

        [SerializeField]
        private RectTransform _frameContainer;

        private List<UIDurableBlock> _list;

        public void Initialize()
        {
            _list = new List<UIDurableBlock>();

            var obj = DataStorage.Instance.GetDataOrNull<GameObject>("UI@DurableBlock");
            _durableBlock = obj.GetComponent<UIDurableBlock>();
        }

        public void CleanUp()
        {
            _list.Clear();
        }

        public void SetData()
        {
            //Boss
            //HQ
            //Durable
        }
    }
}