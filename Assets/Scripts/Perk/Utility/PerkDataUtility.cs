namespace SDefence.Perk
{
    using Usable;
    public class PerkDataUtility
    {
        public static IPerkUsableData Create<T>(int value = 0) where T : IPerkUsableData
        {
            var data = (IPerkUsableData)System.Activator.CreateInstance<T>();
            data.SetPerk(value);
            return data;
        }

        public static IPerkUsableData Create(string type, int value = 0)
        {
            var ptype = System.Type.GetType($"SDefence.Perk.Usable.{type}");
            if (ptype != null)
            {
                var data = (IPerkUsableData)System.Activator.CreateInstance(ptype);
                data.SetPerk(value);
                return data;
            }
#if UNITY_EDITOR
            else
            {
                throw new System.Exception($"{type} is not found");
            }
#endif
        }
    }
}