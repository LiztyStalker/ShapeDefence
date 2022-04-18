namespace PoolSystem
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PoolSystem<T> where T : IPoolElement
    {
        private Queue<T> _queue;

        private System.Func<T> _createCallback = null;
         
        public static PoolSystem<T> Create()
        {
            return new PoolSystem<T>();
        }

        public void Initialize(System.Func<T> createCallback)
        {
            _queue = new Queue<T>();
            _createCallback = createCallback;
        }

        private T CreateInstance()
        {
            Debug.Assert(_createCallback != null, "생성 콜백이 등록되어 있어야 합니다");
            return _createCallback();
        }       
        
        public T GiveElement()
        {
            if(_queue.Count == 0)
            {
                var element = CreateInstance();
                _queue.Enqueue(element);
            }
            return _queue.Dequeue();
        }


        public void RetrieveElement(T element)
        {
            if(!_queue.Contains(element)) _queue.Enqueue(element);
        }

        public void CleanUp()
        {
            _queue.Clear();
        }
    }
}