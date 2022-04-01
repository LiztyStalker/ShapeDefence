namespace Utility.IO
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class SavableData
    {
        [SerializeField]
        private Dictionary<string, object> _children;

        public Dictionary<string, object> Children { get => _children; protected set => _children = value; }

        public static SavableData Create() => new SavableData();

        private SavableData()
        {
            _children = new Dictionary<string, object>();
        }

        public void AddData(string key, object value)
        {
            if (!_children.ContainsKey(key))
            {
                _children.Add(key, value);
            }
            else
            {
                _children[key] = value;
            }
        }

        public T GetValue<T>(string key)
        {
            if (_children != null)
            {
                if (_children.ContainsKey(key))
                {
                    return (T)_children[key];
                }
            }
            throw new System.Exception($"{typeof(T).Name}을 찾을 수 없음");
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}