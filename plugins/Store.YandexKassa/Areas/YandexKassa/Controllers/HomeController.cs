using Microsoft.AspNetCore.Mvc;

namespace Store.YandexKassa.Areas.YandexKassa.Controllers
{
    [Area("YandexKassa")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CallBack()
        {
            return View();
        }
    }
}
