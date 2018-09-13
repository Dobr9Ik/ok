using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Ok.Domain.Core;
using Ok.Domain.Interfaces;
using Ok.Infrastructure.Data;
using Ok.Services.Events;
using Ok.Services.Interfaces;

namespace Ok.Infrastructure.Business
{
    public class NewsSaveService : ISaveNews
    {
        private readonly IRepository<Source> _dbSource;
        private readonly IRepository<News> _dbNews;
        public event Action<object, NewsServiceEvenArg> Message;

        public NewsSaveService()
        {
            _dbSource = new Repository<Source>();
            _dbNews = new Repository<News>();
        }

        public async Task SaveNewsAsync()
        {
            Message?.Invoke(this, new NewsServiceEvenArg("Подождите, сейчас начнется загрузка новостей."));
            var sources = await _dbSource.GetAllAsync();
            if (sources.Any())
            {
                foreach (var source in sources)
                {
                    XDocument document;
                    try
                    {
                        Message?.Invoke(this, new NewsServiceEvenArg($"В обработке {source.Name}."));
                        document = await GetRss(source.Url);
                    }
                    catch
                    {
                        Message?.Invoke(this, new NewsServiceEvenArg("Произошла ошибка! Не удалось считать новости."));
                        continue; // throw;
                    }
                    var news = GetNews(document, source);
                    Message?.Invoke(this, new NewsServiceEvenArg($"Прочитано новостей: {news.Count}."));
                    await SaveAsync(news, source);
                }
            }
            Message?.Invoke(this, new NewsServiceEvenArg("Готово."));
        }

        private async Task SaveAsync(List<News> newNews, Source source)
        {
            int count = 0;
            if (newNews.Any())
            {
                var oldNews = (await _dbSource.GetByIdAsync(source.SourceId, "News")).News;

                foreach (var n in newNews)
                {
                    if (!oldNews.Any(x => x.Title == n.Title && x.Date == n.Date))
                    {
                        count++;
                        await _dbNews.CreateAsync(n);
                    }
                }

                await _dbNews.SaveAsync();
            }
            Message?.Invoke(this, new NewsServiceEvenArg($"Сохранено новостей: {count}\n"));
        }

        private async Task<XDocument> GetRss(string url)
        {
            string xml = string.Empty;
            WebClient client = new WebClient();
            using (var stream = await client.OpenReadTaskAsync(url))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        xml = reader.ReadToEnd();
                    }
                }
            }

            if (xml.StartsWith("\n"))
            {
                xml = xml.Remove(0, 1);
            }

            return XDocument.Parse(xml);
        }

        private List<News> GetNews(XDocument document, Source source)
        {
            List<News> news = new List<News>();

            var items = document?.Root?.Descendants("item").ToList();
            if (items != null && items.Any())
            {
                foreach (var item in items)
                {
                    var title = GetTitle(item);
                    var description = GetDescription(item);
                    DateTime date;

                    try
                    {
                        date = GetDate(item);
                    }
                    catch
                    {
                        continue; //throw;
                    }

                    News n = new News
                    {
                        Title = title,
                        Date = date,
                        Description = description,
                        SourceId = source.SourceId
                    };

                    news.Add(n);
                }
            }

            return news;
        }

        private string GetTitle(XElement item)
        {
            return item.Element("title")?.Value ?? string.Empty;
        }

        private string GetDescription(XElement item)
        {
            return item.Element("description")?.Value ?? string.Empty;
        }

        private DateTime GetDate(XElement item)
        {
            string format = "ddd, dd MMM yyyy HH:mm:ss";
            var dt = item.Element("pubDate")?.Value ?? string.Empty;
            if (!DateTime.TryParse(dt, out DateTime date))
            {
                var index = dt.LastIndexOf(" ", StringComparison.Ordinal);
                if (index > 0)
                {
                    dt = dt.Substring(0, index);
                    return DateTime.ParseExact(dt, format, CultureInfo.CurrentCulture);
                }
            }

            return date;
        }

        public void Dispose()
        {
            _dbNews?.Dispose();
            _dbSource?.Dispose();
        }
    }
}
