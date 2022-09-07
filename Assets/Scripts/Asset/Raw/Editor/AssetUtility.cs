namespace SDefence.Asset
{
    using Usable;
    using System.Collections.Generic;
    using System.Linq;

    #region ##### Utility #####
    public class AssetUtility
    {
        private Dictionary<System.Type, string> _dic;

        private static AssetUtility _current;

        public static AssetUtility Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new AssetUtility();
                }
                return _current;
            }
        }

        public string GetTypeToContext(System.Type type)
        {
            if (_dic.ContainsKey(type))
                return _dic[type];
            return null;
        }

        public System.Type[] GetTypes() => _dic.Keys.ToArray();

        public string[] GetValues() => _dic.Values.ToArray();

        public int FindIndex(System.Type type) => _dic.Keys.ToList().FindIndex(t => t == type);

        private AssetUtility()
        {
            _dic = new Dictionary<System.Type, string>();

            _dic.Add(typeof(NeutralAssetUsableData), "Neutral");
            _dic.Add(typeof(StarAssetUsableData), "Star");
        }
    }
    #endregion
}