using System;
using System.Threading.Tasks;
using Ok.Domain.Core;
using Ok.Domain.Interfaces;
using Ok.Infrastructure.Data;
using Ok.Services.Interfaces;

namespace Ok.Infrastructure.Business
{
    public class PaginationService : IPagination
    {
        private readonly IRepository<News> _dbNews;
        private readonly IRepository<Source> _dbSource;

        public PaginationService()
        {
            _dbNews = new Repository<News>();
            _dbSource = new Repository<Source>();
        }

        public async Task<int> GetCurrentPage(int page, int source, int sizePage)
        {
            var count = source == 0 ? (await _dbNews.GetAllAsync()).Count : (await _dbSource.GetByIdAsync(source, "News"))?.News.Count ?? 0;
            if (page < 1 || page > (int) Math.Ceiling((decimal) count / sizePage))
            {
                return 1;
            }

            return page;
        }

        public async Task<int> GetTotalPage(int source, int sizePage)
        {
            var count = source == 0 ? (await _dbNews.GetAllAsync()).Count : (await _dbSource.GetByIdAsync(source, "News"))?.News.Count ?? 0;
            return (int) Math.Ceiling((decimal) count / sizePage);
        }

        public void Dispose()
        {
            _dbNews?.Dispose();
            _dbSource?.Dispose();
        }
    }
}
