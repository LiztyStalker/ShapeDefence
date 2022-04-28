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
        private int[] _capacity;

        public static OrbitEntity Create() => new OrbitEntity();

        private OrbitEntity()
        {
            _capacity = new int[1];
            _capacity[0] = 1;
        }

        public int Count => _capacity.Length;

        public int GetCapacity(int index) => _capacity[index];

        public void ExpandOrbit(int turretCount)
        {
            var list = new List<int>(_capacity);
            list.Add(turretCount);
            _capacity = list.ToArray();
        }

        #region ##### Savable #####

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

        public bool UpTech(int orbitIndex, int index, string key)
        {
            if (_dic.ContainsKey(orbitIndex))
            {
                var data = (TurretData)DataStorage.Instance.GetDataOrNull<ScriptableObject>(key, "TurretData");
                if (data != null)
                {
                    UpTech(orbitIndex, index, data);
                    return true;
                }
            }
            return false;
        }

        public void UpTech(int orbitIndex, int index, TurretData data)
        {
            _dic[orbitIndex][index].UpTech(data);
            OnUpTechEntityPacketEvent(orbitIndex, index);
            Refresh(orbitIndex, index);
        }

        public void ExpandTurret(int orbitIndex)
        {
            //기본 터렛 필요
            var turret = AdditiveEntity(orbitIndex);
            turret.Initialize(GetDefaultData(), orbitIndex);
            Refresh(orbitIndex);
            OnExpandTurretEntityPacketEvent(orbitIndex);
        }

        public void SetOrbitCount(int orbitCount)
        {
            while(_orbitEntity.Count < orbitCount)
            {
                _orbitEntity.ExpandOrbit(_orbitEntity.Count * 2);
                //ExpandTurret(_orbitEntity.Count - 1);
                ExpandOrbit(_orbitEntity.Count - 1);
                //포탑 제공 미제공?
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
                _defaultTurretData = (TurretData)DataStorage.Instance.GetDataOrNull<ScriptableObject>("Simple", "TurretData");
            }
            return _defaultTurretData;
        }

        private TurretEntity AdditiveEntity(int orbitIndex)
        {
            ExpandOrbit(orbitIndex);

            var turret = TurretEntity.Create();
            _dic[orbitIndex].Add(turret);
            return turret;
        }

        public void Refresh(int orbitIndex)
        {
            OnOrbitEntityPacketEvent();
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


        public void OnOpenTechCommandPacketEvent(int orbitindex, int index)
        {
            var techPacket = new OpenTechEntityPacket();
            var elements = _dic[orbitindex][index].TechRawData.TechRawElements;

            techPacket.Elements = new TechPacketElement[elements.Length];
            for (int i = 0; i < techPacket.Elements.Length; i++)
            {
                var element = new TechPacketElement() { Element = elements[i], IsActiveTech = false };
                techPacket.Elements[i] = element;
            }

            _entityEvent?.Invoke(techPacket);
        }

        public void OnExpandTurretEntityPacket(int orbitIndex)
        {
            var packet = new OpenExpandTurretEntityPacket();
            //Turret Orbit And Index
            packet.OrbitIndex = orbitIndex;
            packet.AssetData = AssetRawData.Create().GetUsableData();
            _entityEvent?.Invoke(packet);
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
                packet.IsExpand = _dic[orbitIndex].Count < _orbitEntity.GetCapacity(orbitIndex);
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
            var packet = new UpTechEntityPacket();
            packet.PastEntity = _dic[orbitIndex][index]; //예전 Entity 데이터 필요
            packet.NowEntity = _dic[orbitIndex][index];
            _entityEvent?.Invoke(packet);
        }

        private void OnExpandTurretEntityPacketEvent(int orbitIndex)
        {
            var packet = new ExpandTurretEntityPacket();
            packet.OrbitIndex = orbitIndex;
            //임시 - Turret 확장 값 필요
            packet.AssetData = AssetRawData.Create().GetUsableData();
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