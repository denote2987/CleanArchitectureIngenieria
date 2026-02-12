using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Interface
{
    public interface IRepositoryBase<T> where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
