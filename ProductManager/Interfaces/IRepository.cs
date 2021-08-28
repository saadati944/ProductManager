using System.Data;

namespace Framework.Interfaces
{
    public delegate void DataChangeHandler();
    public interface IRepository
    {
        DataTable DataTable { get; }

        event DataChangeHandler DataChanged;
        void Update();
    }
}
