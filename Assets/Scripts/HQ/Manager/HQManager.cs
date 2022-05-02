namespace SDefence.HQ
{
    using Entity;
    using Packet;
    using Asset;
    using Utility.IO;
    using Storage;
    using UnityEngine;

    public class HQManager : ISavable
    {
        private readonly string DEFAULT_HQ_KEY = "HQ1";

        private HQEntity _entity;

        public static HQManager Create() => new HQManager();
        public void Initialize()
        {
            _entity = HQEntity.Create();
            _entity.Initialize(GetStorageData(DEFAULT_HQ_KEY));
        }

        public void CleanUp()
        {
            _entity.ClearUpgrade();
        }

        public IAssetUsableData Upgrade()
        {
            var assetData = _entity.GetUpgradeData().Clone();
            _entity.Upgrade();
            Refresh();
            return assetData;
        }

        public bool UpTech(string key)
        {
            var data = GetStorageData(key);
            Debug.Log(data);
            if (data != null)
            {
                UpTech(data);
                return true;
            }
            return false;
        }

        private HQData GetStorageData(string key) => (HQData)DataStorage.Instance.GetDataOrNull<ScriptableObject>(key, "HQData");

        public void UpTech(HQData data)
        {
            _entity.UpTech(data);
            OnUpTechEntityPacketEvent();
            Refresh();
        }

        public void Refresh()
        {
            OnEntityPacketEvent();
        }



        #region ##### Listener #####

        private System.Action<IEntityPacket> _entityEvent;
        public void AddOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent += act;
        public void RemoveOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent -= act;
        private void OnEntityPacketEvent()
        {
            var packet = new HQEntityPacket();
            packet.Entity = _entity;
            packet.IsActiveUpTech = !_entity.TechRawData.IsEmpty() && _entity.IsMaxUpgrade();
            _entityEvent?.Invoke(packet);
        }

        private void OnUpTechEntityPacketEvent()
        {
            var packet = new UpTechEntityPacket();            
            packet.PastEntity = _entity; //예전 Entity 데이터 필요
            packet.NowEntity = _entity;
            _entityEvent?.Invoke(packet);
        }

        public void OnOpenTechCommandPacketEvent()
        {
            var techPacket = new OpenTechEntityPacket();
            var elements = _entity.TechRawData.TechRawElements;

            techPacket.Elements = new TechPacketElement[elements.Length];
            for (int i = 0; i < techPacket.Elements.Length; i++)
            {
                var element = new TechPacketElement() { Element = elements[i], IsActiveTech = false };
                techPacket.Elements[i] = element;
            }

            _entityEvent?.Invoke(techPacket);
        }


        #endregion




        #region ##### Savable #####
        public string SavableKey() => typeof(HQManager).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            data.AddData("Key", _entity.Key);
            data.AddData(_entity.SavableKey(), _entity.GetSavableData());
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            var key = data.GetValue<string>("Key");
            _entity.Initialize(GetStorageData(key));
            _entity.SetSavableData(data.GetValue(_entity.SavableKey()));
        }

        #endregion

    }
}