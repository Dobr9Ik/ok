using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ok.Domain.Core;
using Ok.Infrastructure.Business;
using Ok.Services.Interfaces;
using OkWeb.Models;
using Newtonsoft.Json;
using OkWeb.Services;

namespace OkWeb.Controllers
{
    public class NewsController : Controller
    {
        private readonly int _sizePage;
        private readonly IGetNews _getNews;
        private readonly IPagination _pagination;

        public NewsController()
        {
            _sizePage = GetSizePage();
            _getNews = new NewsGetService(_sizePage);
            _pagination = new PaginationService();
        }

        #region Server form
        public async Task<ActionResult> Index(FormData data)
        {
            data = Helper.CheckFormData(data);
            Pagination p = await GetPaginationAsync(data);
            NewsView news = data != null
                ? new NewsView {Sources = await _getNews.GetSourceNewsAsync(), News = await _getNews.GetNewsAsync(data.Sort, data.Source, p.CurrentPage), Pagination = p, Source = data.Source, Sort = data.Sort}
                : new NewsView {Sources = await _getNews.GetSourceNewsAsync(), News = new List<News>(), Pagination = p, Source = 0, Sort = "date"};
            return View(news);
        }

        [HttpPost]
        public async Task<ActionResult> Index(int source, string sort)
        {
            FormData d = new FormData {Source = source, Sort = sort, Page = 1};
            NewsView news = new NewsView {Sources = await _getNews.GetSourceNewsAsync(), News = await _getNews.GetNewsAsync(sort, source), Pagination = await GetPaginationAsync(d), Source = d.Source, Sort = d.Sort};
            return View(news);
        }
        #endregion

        #region Ajax Form
        public async Task<ActionResult> Ajax()
        {
            var source = await _getNews.GetSourceNewsAsync();
            return View(source);
        }

        [HttpPost]
        public async Task<ActionResult> Ajax(int source, string sort)
        {
            var news = await _getNews.GetAllNewsAsync(sort, source);
            return PartialView("_AjaxForm", news);
        }
        #endregion

        #region Ajax Json
        public async Task<ActionResult> AjaxJson()
        {
            var source = await _getNews.GetSourceNewsAsync();
            return View(source);
        }

        [HttpPost]
        public async Task<JsonResult> AjaxJson(FormData data)
        {
            data = Helper.CheckFormData(data);
            var news = await _getNews.GetNewsAsync(data.Sort, data.Source, data.Page);
            JsonSerializerSettings s = new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            var result = JsonConvert.SerializeObject(news, s);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> PaginationJson(FormData data)
        {
            data = Helper.CheckFormData(data);
            Pagination p = await GetPaginationAsync(data);
            return Json(p, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Helper
        private async Task<Pagination> GetPaginationAsync(FormData data)
        {
            if (data == null)
            {
                return new Pagination();
            }
            
            return new Pagination
            {
                SizePage = _sizePage,
                CurrentPage = await _pagination.GetCurrentPage(data.Page, data.Source, _sizePage),
                TotalPages = await _pagination.GetTotalPage(data.Source, _sizePage),
                Sort = data.Sort ?? "date",
                Source = data.Source
            };
        }

        private int GetSizePage()
        {
            try
            {
                return int.Parse(ConfigurationManager.AppSettings["PageCount"]);

            }
            catch
            {
                return 10;
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _getNews?.Dispose();
            _pagination?.Dispose();
            base.Dispose(disposing);
        }
    }
}