namespace SDefence.Durable.Usable
{
    public class UniversalDurableUsableData : AbstractDurableUsableData, IDurableUsableData
    {
        public static UniversalDurableUsableData Create(int value) => new UniversalDurableUsableData(value);

        private UniversalDurableUsableData(int value)
        {
            Value = value;
        }
    }
}