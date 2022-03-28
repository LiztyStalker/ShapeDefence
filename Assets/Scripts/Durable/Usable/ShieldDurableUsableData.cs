namespace SDefence.Durable.Usable
{
    public class ShieldDurableUsableData : AbstractDurableUsableData, IDurableUsableData, ILimitedDurable
    {
        public IDurableUsableData GetDurableUsableCase()
        {
            return DurableUsableCase.Create(this);
        }
    }
}