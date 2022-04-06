namespace SDefence.Turret
{
    using Entity;
    using Packet;
    using System.Collections.Generic;
    using Utility.IO;

    public class TurretManager : ISavable
    {
        private List<TurretEntity> _list;
        // 0 - Main Turret
        // 


        public static TurretManager Create() => new TurretManager();
        public void Initialize()
        {
            _list = new List<TurretEntity>();

            var turret = AdditiveEntity();

            //기본 터렛 필요
            turret.Initialize(TurretData.Create(), 0);
        }
        
        public void CleanUp()
        {
            for(int i = 0; i < _list.Count; i++)
            {
                _list[i].CleanUp();
            }
            _list.Clear();
        }

        public void Upgrade(int index)
        {
            _list[index].Upgrade();
            Refresh(index);
        }

        public void UpTech(int index, TurretData data)
        {
            _list[index].UpTech(data);
            Refresh(index);
        }

        public void Expand(int orbitIndex)
        {
            //기본 터렛 필요
            var turret = AdditiveEntity();
            turret.Initialize(TurretData.Create(), orbitIndex);
            Refresh(_list.Count - 1);
        }

        private TurretEntity AdditiveEntity()
        {
            var turret = TurretEntity.Create();
            _list.Add(turret);
            return turret;
        }

        public void Refresh()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                Refresh(i);
            }
        }

        private void Refresh(int index)
        {
            OnEntityPacketEvent(index, _list[index]);
        }



        #region ##### Listener #####

        private System.Action<IEntityPacket> _entityEvent;
        public void AddOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent += act;
        public void RemoveOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent -= act;
        private void OnEntityPacketEvent(int index, TurretEntity entity) 
        {
            var packet = new TurretEntityPacket();
            packet.SetData(index, entity);
            _entityEvent?.Invoke(packet);
        }


        #endregion




        #region ##### Savable #####
        public string SavableKey() => typeof(TurretManager).Name;

        public SavableData GetSavableData()
        {
            var arr = new KeyValuePair<string, SavableData>[_list.Count];
            for (int i = 0; i < _list.Count; i++) 
            {
                var entity = _list[i];
                arr[i] = new KeyValuePair<string, SavableData>(entity.Key, entity.GetSavableData());
            }
            var data = SavableData.Create();
            data.AddData(SavableKey(), arr);
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            var arr = data.GetValue<KeyValuePair<string, SavableData>[]>(SavableKey());
            for (int i = 0; i < arr.Length; i++)
            {
                if(i >= _list.Count)
                {
                    AdditiveEntity();
                }

                var savable = arr[i];
                // 포탑 데이터 불러오기
                //_list[i].SetData(savable.Key);
                _list[i].SetSavableData(savable.Value);
            }
        }

        #endregion

    }
}