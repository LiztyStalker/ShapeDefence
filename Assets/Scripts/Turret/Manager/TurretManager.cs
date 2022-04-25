namespace SDefence.Turret
{
    using Entity;
    using Packet;
    using Asset;
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

        private Dictionary<int, List<TurretEntity>> _dic;

        private TurretData _defaultTurretData;

        public static TurretManager Create() => new TurretManager();
        public void Initialize()
        {
            _orbitEntity = OrbitEntity.Create();
            _dic = new Dictionary<int, List<TurretEntity>>();

            var turret = AdditiveEntity(0);

            //기본 터렛 필요
            turret.Initialize(GetDefaultData(), 0);
        }
        
        public void CleanUp()
        {
            foreach(var key in _dic.Keys)
            {
                for (int i = 0; i < _dic[key].Count; i++)
                {
                    _dic[key][i].CleanUp();
                }
            }

            _dic.Clear();
            _orbitEntity = null;
        }

        public IAssetUsableData Upgrade(int orbitIndex, int index)
        {
            if (_dic.ContainsKey(orbitIndex))
            {
                var entity = _dic[orbitIndex][index];
                var assetData = entity.GetUpgradeData().Clone();
                _dic[orbitIndex][index].Upgrade();
                Refresh(orbitIndex, index);
                return assetData;
            }
            return null;
        }

        public void UpTech(int orbitIndex, int index, TurretData data)
        {
            if (_dic.ContainsKey(orbitIndex))
            {
                _dic[orbitIndex][index].UpTech(data);
                Refresh(orbitIndex, index);
            }
        }

        public void Expand(int orbitIndex)
        {
            //기본 터렛 필요
            var turret = AdditiveEntity(orbitIndex);
            turret.Initialize(GetDefaultData(), orbitIndex);
            Refresh(orbitIndex);
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

        private TurretEntity AdditiveEntity(int orbitIndex)
        {
            if (!_dic.ContainsKey(orbitIndex))
            {
                _dic.Add(orbitIndex, new List<TurretEntity>());
            }

            var turret = TurretEntity.Create();
            _dic[orbitIndex].Add(turret);
            return turret;
        }

        public void Refresh(int orbitIndex)
        {
            if (_dic.ContainsKey(orbitIndex))
            {
                OnEntityPacketEvent(orbitIndex);
            }
        }

        private void Refresh(int orbitIndex, int index)
        {
            if (_dic.ContainsKey(orbitIndex))
            {
                OnEntityPacketEvent(orbitIndex, index, _dic[orbitIndex][index]);
            }

        }


        //public void OnCommandPacketEvent(TurretCommandPacket packet)
        //{
        //    if (packet.IsUpgrade) Upgrade(packet.Index);
        //    if (packet.IsExpand) Expand(packet.OrbitIndex);
        //}


        #region ##### Listener #####

        private System.Action<IEntityPacket> _entityEvent;
        public void AddOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent += act;
        public void RemoveOnEntityPacketListener(System.Action<IEntityPacket> act) => _entityEvent -= act;
        private void OnEntityPacketEvent(int orbitIndex, int index, TurretEntity entity) 
        {
            var packet = CreateEntityPacket(orbitIndex, index, entity);
            _entityEvent?.Invoke(packet);
        }

        private void OnEntityPacketEvent(int orbitIndex)
        {
            var packet = new TurretArrayEntityPacket();
            if (_dic.ContainsKey(orbitIndex))
            {
                var arr = new TurretEntityPacket[_dic[orbitIndex].Count];
                for(int i = 0; i < _dic[orbitIndex].Count; i++)
                {
                    var trPacket = CreateEntityPacket(orbitIndex, i, _dic[orbitIndex][i]);
                    arr[i] = trPacket;
                }
                packet.packets = arr;
            }
            _entityEvent?.Invoke(packet);
        }

        private TurretEntityPacket CreateEntityPacket(int orbitIndex, int index, TurretEntity entity)
        {
            var packet = new TurretEntityPacket();
            packet.Entity = entity;
            packet.OrbitIndex = orbitIndex;
            packet.Index = index;
            return packet;
        }


        #endregion




        #region ##### Savable #####
        public string SavableKey() => typeof(TurretManager).Name;

        public SavableData GetSavableData()
        {
            var arr = new KeyValuePair<int, object>[_dic.Count];
            foreach(var key in _dic.Keys)
            {

                var list = _dic[key];
                var entityArr = new KeyValuePair<string, SavableData>[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    var entity = list[i];
                    entityArr[i] = new KeyValuePair<string, SavableData>(entity.Key, entity.GetSavableData());
                }
                arr[key] = new KeyValuePair<int, object>(key, entityArr);
            }

            var data = SavableData.Create();
            data.AddData(SavableKey(), arr);
            data.AddData(_orbitEntity.SavableKey(), _orbitEntity.GetSavableData());
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            _orbitEntity.SetSavableData(data.GetValue(_orbitEntity.SavableKey()));

            var arr = data.GetValue<KeyValuePair<int, object>[]>(SavableKey());
            for (int i = 0; i < arr.Length; i++)
            {
                var obj = arr[i];
                var entityArr = (KeyValuePair<string, SavableData>[])obj.Value;
                for (int j = 0; j < entityArr.Length; j++)
                {
                    if (!_dic.ContainsKey(obj.Key))
                    {
                        _dic.Add(obj.Key, new List<TurretEntity>());
                    }

                    var list = _dic[obj.Key];
                    var turret = TurretEntity.Create();
                    turret.SetSavableData(entityArr[j].Value);
                    list.Add(turret);
                }
            }

        }

        #endregion

    }
}