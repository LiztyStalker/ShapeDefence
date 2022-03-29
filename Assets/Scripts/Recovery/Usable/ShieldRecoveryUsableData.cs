namespace SDefence.Recovery.Usable
{
    using Durable.Usable;
    public class ShieldRecoveryUsableData : AbstractRecoveryUsableData, IRecoveryUsableData
    {
        public override string DurableKey() => typeof(ShieldDurableUsableData).Name;

    }
}