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

        public SavableData()
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
            return System.Activator.CreateInstance<T>();
        }

        public SavableData GetValue(string key)
        {
            if (_children != null)
            {
                if (_children.ContainsKey(key))
                {
                    return (SavableData)_children[key];
                }
            }
            return null;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}