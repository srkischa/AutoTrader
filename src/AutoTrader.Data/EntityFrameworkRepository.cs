using AutoTrader.DomainModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AutoTrader.Data
{
    public class EntityFrameworkRepository<T> : IRepository<T> where T : Entity
    {
        private readonly DbContext _context;

        public EntityFrameworkRepository(DbContext context)
        {
            if (context == null) throw new ArgumentException(nameof(context));

            _context = context;
            DbSet = context.Set<T>();
        }

        protected DbContext DbContext => _context;

        protected DbSet<T> DbSet { get; }

        public IQueryable<T> Items => DbSet;

        public void AddRange(IEnumerable<T> items)
        {
            DbSet.AddRange(items);
        }

        public void Delete(int id)
        {
            var item = FindById(id);
            if (item != null)
            {
                DbSet.Remove(item);
            }
        }

        public void Delete(T item)
        {
            DbSet.Remove(item);
        }

        public T FindById(int id)
        {
            return DbSet.Find(id);
        }

        public void SaveOrUpdate(T item)
        {
            if(item.Id == 0)
            {
                DbSet.Add(item);
            }
            else
            {
                _context.Entry(item).State = EntityState.Modified;
            }
        }
    }
}
