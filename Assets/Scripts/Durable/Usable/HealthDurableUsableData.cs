namespace SDefence.Durable.Usable
{
    public class HealthDurableUsableData : AbstractDurableUsableData, IDurableUsableData, ILimitedDurable
    {
        public IDurableUsableData GetDurableUsableCase()
        {
            return DurableUsableCase.Create(this);
        }
    }
}