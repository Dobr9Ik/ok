using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ok.Domain.Interfaces
{
    public interface IRepository<T>: IDisposable where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(string include);
        //Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(int id, string include);
        Task CreateAsync(T t);
        Task UpdateAsync(T t);
        Task DeleteAsync(int id);
        Task<bool> SaveAsync();
    }
}