using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Ok.Domain.Interfaces;

namespace Ok.Infrastructure.Data
{
    public class Repository<T>:IRepository<T> where T : class
    {
        private readonly ApplicationContext _db;

        public Repository()
        {
            _db = new ApplicationContext();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(string include)
        {
            return await _db.Set<T>().Include(include).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task<T> GetByIdAsync(int id, string include)
        {
            var result = await _db.Set<T>().FindAsync(id);
            if (result != null)
            {
                await _db.Entry(result).Collection(include).LoadAsync();
            }
            return result; 
        }

        public async Task CreateAsync(T t)
        {
            await Task.Factory.StartNew(() =>
            {
                _db.Set<T>().Add(t);
            });
        }

        public async Task UpdateAsync(T t)
        {
            await Task.Factory.StartNew(() =>
            {
                _db.Set<T>().Attach(t);
                _db.Entry(t).State = EntityState.Modified;
            });
        }

        public async Task DeleteAsync(int id)
        {
            await Task.Factory.StartNew(() =>
            {
                var entity = _db.Set<T>().Find(id);
                if (entity != null)
                {
                    _db.Set<T>().Remove(entity);
                }
            });
        }

        public async Task<bool> SaveAsync()
        {
            var result = await _db.SaveChangesAsync();
            return result != 0;
        }

        private bool _disposed;
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
