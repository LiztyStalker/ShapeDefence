namespace Utility.Number
{
    public interface INumberData
    {
        string ToString();
        INumberData Clone();
        void CleanUp();
    }
}