namespace SDefence.Turret
{
    using Entity;
    using Packet;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine;
    using Utility.IO;

    public class OrbitEntity : ISavable
    {
        private int[] _capacity;

        public static OrbitEntity Create() => new OrbitEntity();

        private OrbitEntity()
        {
            _capacity = new int[1];
            _capacity[0] = 1;
        }

        public int GetCapacity(int index) => _capacity[index];

        public void ExpandOrbit(int turretCount)
        {
            var list = new List<int>(_capacity);
            list.Add(turretCount);
            _capacity = list.ToArray();
        }

        public string SavableKey() => typeof(OrbitEntity).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            for (int i = 0; i < _capacity.Length; i++) 
            {
                data.AddData(i.ToString(), _capacity[i]);
            }
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            _capacity = new int[data.Children.Count];
            foreach(var key in data.Children.Keys)
            {
                var index = int.Parse(key);
                _capacity[index] = (int)data.Children[key];
            }
        }
    }


    public class TurretManager : ISavable
    {
        private OrbitEntity _orbitEntity;

        private List<TurretEntity> _list;

        private TurretData _defaultTurretData;

        public static TurretManager Create() => new TurretManager();
        public void Initialize()
        {
            _orbitEntity = OrbitEntity.Create();
            _list = new List<TurretEntity>();

            var turret = AdditiveEntity();

            //기본 터렛 필요
            turret.Initialize(GetDefaultData(), 0);
        }
        
        public void CleanUp()
        {
            for(int i = 0; i < _list.Count; i++)
            {
                _list[i].CleanUp();
            }
            _list.Clear();
            _orbitEntity = null;
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
            turret.Initialize(GetDefaultData(), orbitIndex);
            Refresh(_list.Count - 1);
        }

        public void ExpandOrbit(int turretCount)
        {
            _orbitEntity.ExpandOrbit(turretCount);
        }

        private TurretData GetDefaultData()
        {
            if(_defaultTurretData == null)
            {
                _defaultTurretData = (TurretData)DataStorage.Instance.GetDataOrNull<ScriptableObject>("Simple", "TurretData");
            }
            return _defaultTurretData;
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


        public void OnCommandPacketEvent(TurretCommandPacket packet)
        {
            if (packet.IsUpgrade) Upgrade(packet.Index);
            if (packet.IsExpand) Expand(packet.OrbitIndex);
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
            data.AddData(_orbitEntity.SavableKey(), _orbitEntity.GetSavableData());
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

            _orbitEntity.SetSavableData(data.GetValue(_orbitEntity.SavableKey()));
        }

        #endregion

    }
}