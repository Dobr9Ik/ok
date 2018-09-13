using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ok.Services.Interfaces
{
    public interface IPagination : IDisposable
    {
        Task<int> GetCurrentPage(int page, int source, int sizePage);
        Task<int> GetTotalPage(int source, int sizePage);
        List<int> GetViewPagination(int totalPage, int currentPage);
    }
}