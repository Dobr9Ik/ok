using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ok.Domain.Core;
using Ok.Domain.Interfaces;
using Ok.Infrastructure.Data;
using Ok.Services.Interfaces;

namespace Ok.Infrastructure.Business
{
    public class NewsGetService : IGetNews
    {
        private readonly int _countNews;
        private readonly IRepository<News> _dbNews;
        private readonly IRepository<Source> _dbSource;

        public NewsGetService(int countNews)
        {
            _dbNews = new Repository<News>();
            _dbSource = new Repository<Source>();
            _countNews = countNews;
        }

        public async Task<List<Source>> GetSourceNewsAsync()
        {
            var sources = await _dbSource.GetAllAsync();
            List<Source> sourcesList = new List<Source>(sources.Count + 1) {new Source {Name = "Все", SourceId = 0}};
            sourcesList.AddRange(sources);
            return sourcesList;
        }

        public async Task<List<News>> GetNewsAsync(string sort, int sourceId, int page = 1)
        {
            List<News> newsList = new List<News>();
            
            var news = sourceId == 0 ? await _dbNews.GetAllAsync("Source") : (await _dbSource.GetByIdAsync(sourceId, "News"))?.News;
            if (news != null)
            {
                switch (sort)
                {
                    case "date":
                        newsList.AddRange(news.OrderByDescending(x => x.Date).ToList());
                        break;
                    case "source":
                        newsList.AddRange(news.OrderByDescending(x => x.Source.Name).ToList());
                        break;
                }
                return newsList.Skip((page - 1) * _countNews).Take(_countNews).ToList();
            }
            return newsList;
        }

        public async Task<List<News>> GetAllNewsAsync(string sort, int sourceId)
        {
            var news = sourceId == 0 ? await _dbNews.GetAllAsync("Source") : (await _dbSource.GetByIdAsync(sourceId, "News"))?.News.ToList();
            switch (sort)
            {
                case "date":
                    return news?.OrderByDescending(x => x.Date).ToList();
                case "source":
                    return news?.OrderByDescending(x => x.Source.Name).ToList();
            }
            return news;
        }

        public void Dispose()
        {
            _dbNews?.Dispose();
            _dbSource?.Dispose();
        }
    }
}
