namespace SDefence.Turret
{
    using Entity;
    using Packet;
    using Asset;
    using Storage;
    using System.Collections.Generic;
    using UnityEngine;
    using Utility.IO;
    using Asset.Raw;

    public class OrbitEntity : ISavable
    {
        private const int ORBIT_ASSET = 1000;
        private const int EXPAND_ASSET = 1000;

        private Dictionary<int, int> _capacity;

        public static OrbitEntity Create() => new OrbitEntity();

        private OrbitEntity()
        {
            _capacity = new Dictionary<int, int>();
        }

        public int Count => _capacity.Count;

        public int GetCapacity(int orbitIndex) => _capacity[orbitIndex];
        public bool HasOrbitIndex(int orbitIndex) => _capacity.ContainsKey(orbitIndex);
        public void ExpandOrbit(int orbitIndex, int turretCount)
        {
            if (!_capacity.ContainsKey(orbitIndex))
            {
                _capacity.Add(orbitIndex, turretCount);
            }
        }

        public IAssetUsableData GetAssetUsableData(int orbitIndex, int expandCount)
        {
            var asset = AssetRawData.Create();
            asset.SetData("Neutral", (orbitIndex * ORBIT_ASSET).ToString(), EXPAND_ASSET.ToString(), "0");
            return asset.GetUsableData(expandCount - 1);
        }

        #region ##### Savable #####

        public string SavableKey() => typeof(OrbitEntity).Name;

        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            foreach(var key in _capacity.Keys)
            {
                data.AddData(key.ToString(), _capacity[key]);
            }
            return data;
        }

        public void SetSavableData(SavableData data)
        {
            foreach(var key in data.Children.Keys)
            {
                ExpandOrbit(int.Parse(key), (int)data.Children[key]);
            }
        }        
        #endregion
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

            var turret = AddTurret(0);

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
                var assetData = entity.GetUpgradeAssetUsableData().Clone();
                _dic[orbitIndex][index].Upgrade();
                Refresh(orbitIndex, index);
                return assetData;
            }
            return null;
        }

        public bool UpTech(int orbitIndex, int index, string key, IAssetUsableData techAssetData)
        {
            if (_dic.ContainsKey(orbitIndex))
            {
                var data = GetStorageData(key);
                if (data != null)
                {
                    UpTech(orbitIndex, index, data, techAssetData);
                    return true;
                }
            }
            return false;
        }

        public void UpTech(int orbitIndex, int index, TurretData data, IAssetUsableData techAssetData)
        {
            _dic[orbitIndex][index].UpTech(data, techAssetData);
            OnUpTechEntityPacketEvent(orbitIndex, index);
            Refresh(orbitIndex, index);
        }

        public IAssetUsableData ExpandTurret(int orbitIndex)
        {

            //Turret Create
            var turret = AddTurret(orbitIndex);
            turret.Initialize(GetDefaultData(), orbitIndex);

            //Create Subject AssetData
            //Count = turret + 1 - 1
            var assetData = _orbitEntity.GetAssetUsableData(orbitIndex, _dic[orbitIndex].Count - 1);

            //EntityPacket
            Refresh(orbitIndex);
            OnExpandEntityPacketEvent(orbitIndex);

            return assetData;
        }

        public void SetOrbitCount(int orbitIndex, int capacity)
        {
            if (!_orbitEntity.HasOrbitIndex(orbitIndex))
            {
                _orbitEntity.ExpandOrbit(orbitIndex, capacity);
                ExpandTurret(orbitIndex); //포탑 제공
                                          //ExpandOrbit(_orbitEntity.Count - 1); //포탑 미제공
            }
            OnOrbitEntityPacketEvent();
        }

        private void ExpandOrbit(int orbitIndex)
        {
            if (!_dic.ContainsKey(orbitIndex))
            {
                _dic.Add(orbitIndex, new List<TurretEntity>());
            }
        }

        private TurretData GetDefaultData()
        {
            if(_defaultTurretData == null)
            {
                _defaultTurretData = GetStorageData("Simple");
            }
            return _defaultTurretData;
        }

        private TurretData GetStorageData(string key) => (TurretData)DataStorage.Instance.GetDataOrNull<ScriptableObject>(key, "TurretData");

        private TurretEntity AddTurret(int orbitIndex)
        {
            ExpandOrbit(orbitIndex);

            var turret = TurretEntity.Create();
            _dic[orbitIndex].Add(turret);
            return turret;
        }

        public void RefreshAll()
        {
            OnOrbitEntityPacketEvent();
            for (int i = 0; i < _dic.Count; i++) 
            { 
                OnEntityPacketEvent(i);
            }
        }

        public void Refresh(int orbitIndex)
        {
            OnOrbitEntityPacketEvent();
            if (_dic.ContainsKey(orbitIndex))
            {
                OnEntityPacketEvent(orbitIndex);
                OnExpandEntityPacketEvent(orbitIndex);
            }
        }

        private void Refresh(int orbitIndex, int index)
        {
            if (_dic.ContainsKey(orbitIndex))
            {
                OnEntityPacketEvent(orbitIndex, index, _dic[orbitIndex][index]);
            }
        }


        public void OnOpenTechCommandPacketEvent(int orbitindex, int index)
        {
            var packet = new OpenTechEntityPacket();
            var elements = _dic[orbitindex][index].TechRawData.TechRawElements;

            packet.Elements = new TechPacketElement[elements.Length];
            packet.OrbitIndex = orbitindex;
            packet.Index = index;

            for (int i = 0; i < packet.Elements.Length; i++)
            {
                var element = new TechPacketElement() { Element = elements[i], IsActiveTech = false };
                packet.Elements[i] = element;
            }

            _entityEvent?.Invoke(packet);
        }

        public void OnOpenExpandTurretEntityPacketEvent(int orbitIndex)
        {
            var packet = new OpenExpandTurretEntityPacket();
            //Turret Orbit And Index
            packet.OrbitIndex = orbitIndex;
            packet.AssetData = _orbitEntity.GetAssetUsableData(orbitIndex, _dic[orbitIndex].Count);
            _entityEvent?.Invoke(packet);
        }

        public void OnOpenDisassembleTurretEntityPacket(int orbitIndex, int index)
        {
            var packet = new OpenDisassembleEntityPacket();
            //Asset
            packet.AssetEntity = _dic[orbitIndex][index].DisassembleAssetEntity;
            packet.OrbitIndex = orbitIndex;
            packet.Index = index; 
            _entityEvent?.Invoke(packet);

        }

        public void OnDisassembleTurretEntityPacket(int orbitIndex, int index)
        {
            _dic[orbitIndex][index].Initialize(GetDefaultData(), orbitIndex);

            OnEntityPacketEvent(orbitIndex, index, _dic[orbitIndex][index]);

            //var packet = new TurretEntityPacket();
            //packet.nowData = _dic[orbitIndex][index];
            //packet.pastData = _dic[orbitIndex][index];
            //_entityEvent?.Invoke(packet);
        }


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
            if (orbitIndex == 0)
            {
                OnEntityPacketEvent(orbitIndex, 0, _dic[orbitIndex][0]);
            }
            else
            {
                var packet = new TurretArrayEntityPacket();
                if (_dic.ContainsKey(orbitIndex))
                {
                    var arr = new TurretEntityPacket[_dic[orbitIndex].Count];
                    for (int i = 0; i < _dic[orbitIndex].Count; i++)
                    {
                        var trPacket = CreateEntityPacket(orbitIndex, i, _dic[orbitIndex][i]);
                        arr[i] = trPacket;
                    }
                    packet.packets = arr;
                }
                packet.OrbitIndex = orbitIndex;
                packet.TurretCapacity = _orbitEntity.GetCapacity(orbitIndex);
                packet.TurretCount = _dic[orbitIndex].Count;

                _entityEvent?.Invoke(packet);
            }
        }
        private TurretEntityPacket CreateEntityPacket(int orbitIndex, int index, TurretEntity entity)
        {
            var packet = new TurretEntityPacket();
            packet.Entity = entity;
            packet.OrbitIndex = orbitIndex;
            packet.Index = index;
            packet.IsActiveUpTech = !entity.TechRawData.IsEmpty() && entity.IsMaxUpgrade();
            return packet;
        }

        private void OnOrbitEntityPacketEvent()
        {
            var packet = new TurretOrbitEntityPacket();
            packet.OrbitCount = _orbitEntity.Count;
            _entityEvent?.Invoke(packet);
        }
                
        private void OnUpTechEntityPacketEvent(int orbitIndex, int index)
        {
            Debug.Log(orbitIndex + " " + index);
            var packet = new UpTechEntityPacket();
            packet.PastEntity = _dic[orbitIndex][index]; //예전 Entity 데이터 필요
            packet.NowEntity = _dic[orbitIndex][index];
            _entityEvent?.Invoke(packet);
        }

        private void OnExpandEntityPacketEvent(int orbitIndex)
        {
            var packet = new TurretExpandEntityPacket();
            packet.IsMaxExpand = _dic[orbitIndex].Count >= _orbitEntity.GetCapacity(orbitIndex);
            packet.ExpandAssetData = _orbitEntity.GetAssetUsableData(orbitIndex, _dic[orbitIndex].Count);
            _entityEvent?.Invoke(packet);
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
            for (int orbitIndex = 0; orbitIndex < arr.Length; orbitIndex++)
            {
                var obj = arr[orbitIndex];
                var entityArr = (KeyValuePair<string, SavableData>[])obj.Value;

                for (int turretIndex = 0; turretIndex < entityArr.Length; turretIndex++)
                {
                    if (!_dic.ContainsKey(obj.Key))
                    {
                        _dic.Add(obj.Key, new List<TurretEntity>());
                    }

                    var key = entityArr[turretIndex].Key;

                    if (orbitIndex == 0 && turretIndex == 0)
                    {
                        _dic[orbitIndex][turretIndex].Initialize(GetStorageData(key), obj.Key);
                        _dic[orbitIndex][turretIndex].SetSavableData(entityArr[turretIndex].Value);
                    }
                    else
                    {
                        var list = _dic[obj.Key];
                        var turret = TurretEntity.Create();
                        turret.Initialize(GetStorageData(key), obj.Key);
                        turret.SetSavableData(entityArr[turretIndex].Value);
                        list.Add(turret);
                    }
                }
            }

        }

#endregion

    }
}