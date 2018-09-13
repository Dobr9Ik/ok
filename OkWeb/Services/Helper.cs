using OkWeb.Models;

namespace OkWeb.Services
{
    public class Helper
    {
        public static FormData CheckFormData(FormData data)
        {
            if (data.Source == 0 && data.Page == 0 && data.Sort == null)
            {
                return null;
            }
            return new FormData {Source = data.Source < 0 ? 0 : data.Source, Page = data.Page <= 0 ? 1 : data.Page, Sort = string.IsNullOrEmpty(data.Sort) ? "date" : data.Sort};
        }
    }
}