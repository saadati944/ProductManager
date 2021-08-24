using System.Data;

namespace Business.Repositories
{
    public delegate void DataChangeHandler();
    public interface IRepository
    {
        DataTable DataTable { get; }


        event DataChangeHandler DataChanged;
        void Update();
    }
}
