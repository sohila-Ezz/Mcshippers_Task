using System.Collections.Generic;

namespace Mcshippers_Task.Repository
{
    public interface IRepository <T,T1>
    {
        List<T> GetAll();
        int Delete(T1 id);
        T GetById(T1 id);
        int Insert(T item);
        int Update(T1 id, T item);
    }
}
