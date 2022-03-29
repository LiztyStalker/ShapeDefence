namespace SDefence.Recovery.Usable
{
    using Durable.Usable;

    public class HealthRecoveryUsableData : AbstractRecoveryUsableData, IRecoveryUsableData
    {
        public override string DurableKey() => typeof(HealthDurableUsableData).Name;
    }
}