

namespace SDefence.Perk.Usable
{
    using Utility.IO;
    public interface IPerkUsableData : ISavable
    {
        public int GetValue();
        public void SetPerk(int value);
        public void SetPerk(IPerkUsableData data);
        public void AddPerk(int value);
        public void AddPerk(IPerkUsableData data);
    }

    public abstract class AbstractPerkUsableData 
    {
        private int _value;

        public int Value { get => _value; protected set => _value = value; }

        public void SetPerk(IPerkUsableData data) => _value = ((AbstractPerkUsableData)data)._value;
        public void SetPerk(int value) => _value = value;


        public void AddPerk(IPerkUsableData data) => _value += ((AbstractPerkUsableData)data)._value;
        public void AddPerk(int value) => _value += value;



        public int GetValue() => _value;
        public SavableData GetSavableData()
        {
            var data = SavableData.Create();
            data.AddData(SavableKey(), _value);
            return data;
        }
        public abstract string SavableKey();
        public void SetSavableData(SavableData data) => Value = data.GetValue<int>(SavableKey());
    }

    public class HealthPerkUsableData : AbstractPerkUsableData, IPerkUsableData
    {
        public override string SavableKey() => typeof(HealthPerkUsableData).Name;
    }
    //public class ArmorPerkUsableData : AbstractPerkUsableData, IPerkUsableData 
    //{ 

    //}
}