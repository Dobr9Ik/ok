using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ok.Domain.Core;

namespace Ok.Services.Interfaces
{
    public interface IGetNews : IDisposable
    {
        Task<List<Source>> GetSourceNewsAsync();
        Task<List<News>> GetNewsAsync(string sort, int sourceId, int page = 1);
        Task<List<News>> GetAllNewsAsync(string sort, int sourceId);
    }
}