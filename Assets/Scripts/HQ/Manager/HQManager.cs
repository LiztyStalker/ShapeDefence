namespace SDefence.HQ
{
    using Entity;
    using Packet;
    using Utility.IO;

    public class HQManager : ISavable
    {
        private HQEntity _entity;

        public static HQManager Create() => new HQManager();
        public void Initialize()
        {
            _entity = HQEntity.Create();
            //기본 HQData 가져오기
            _entity.Initialize(HQData.Create());
        }
        
        public void CleanUp()
        {
            _entity.ClearUpgrade();
        }

        public void Upgrade()
        {
            _entity.Upgrade();
            Refresh();
        }

        public void UpTech(HQData data)
        {
            _entity.UpTech(data);
            Refresh();
        }

        public void Refresh()
        {
            OnEntityPacketEvent();
        }

        public void OnCommandPacketEvent(HQCommandPacket packet)
        {
            if (packet.IsUpgrade) Upgrade();
        }


        #region ##### Listener #####

        private System.Action<IEntityPacket> _entityEvent;
        public void AddOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent += act;
        public void RemoveOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent -= act;
        private void OnEntityPacketEvent()
        {
            var packet = new HQEntityPacket();
            packet.SetData(_entity);
            _entityEvent?.Invoke(packet);
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
            //StorageManager에서 HQData 가져오기

            _entity.SetSavableData(data.GetValue(_entity.SavableKey()));
        }

        #endregion

    }
}