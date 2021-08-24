using System.Data;

namespace DataLayer.Repositories
{
    public delegate void DataChangeHandler();
    public interface IRepository
    {
        DataTable DataTable { get; }


        event DataChangeHandler DataChanged;
        void Update();
    }
}
