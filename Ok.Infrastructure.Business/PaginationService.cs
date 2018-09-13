using System;
using System.Collections.Generic;
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

        public List<int> GetViewPagination(int totalPage, int currentPage)
        {
            List<int> list = new List<int>();
            if (totalPage > 9)
            {
                list.Add(1);

                if (currentPage - 2 > 2)
                {
                    list.Add(-1);
                    for (int i = 2, j = currentPage - 2; i <= 4; i++, j++)
                    {
                        list.Add(j);
                    }
                }
                else
                {
                    for (int i = 2; i <= currentPage; i++)
                    {
                        list.Add(i);
                    }
                }

                if (currentPage + 2 < totalPage - 1)
                {
                    for (int i = currentPage + 1; i <= currentPage + 2; i++)
                    {
                        list.Add(i);
                    }

                    list.Add(-1);
                    list.Add(totalPage);
                }
                else
                {
                    for (int i = currentPage + 1; i <= totalPage; i++)
                    {
                        list.Add(i);
                    }
                }
            }
            else
            {
                for (int i = 0, j = 1; i < totalPage; i++, j++)
                {
                    list.Add(j);
                }
            }

            return list;
        }

        public void Dispose()
        {
            _dbNews?.Dispose();
            _dbSource?.Dispose();
        }
    }
}
