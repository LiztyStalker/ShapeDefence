namespace SDefence.Asset.Usable
{
    public class NeutralAssetUsableData : AbstractAssetUsableData, IAssetUsableData
    {
        //NeutralAssetUsableData
        public override string SavableKey() => typeof(NeutralAssetUsableData).AssemblyQualifiedName;
    }
}