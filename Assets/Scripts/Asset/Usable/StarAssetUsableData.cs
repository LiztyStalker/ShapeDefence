namespace SDefence.Asset.Usable
{
    public class StarAssetUsableData : AbstractAssetUsableData, IAssetUsableData
    {
        //StarAssetUsableData
        public override string SavableKey() => typeof(StarAssetUsableData).AssemblyQualifiedName;

    }
}