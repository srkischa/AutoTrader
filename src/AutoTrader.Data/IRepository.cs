using AutoTrader.DomainModel;
using System.Collections.Generic;
using System.Linq;

namespace AutoTrader.Data
{
    public interface IRepository<T> where T : Entity
    {
        IQueryable<T> Items { get; }
        void SaveOrUpdate(T item);
        void AddRange(IEnumerable<T> items);
        void Delete(T item);
        void Delete(int id);
        T FindById(int id);
    }
}
