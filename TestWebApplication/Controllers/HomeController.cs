using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace TestWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IHtmlLocalizer<HomeController> _htmlLocalizer;

        public HomeController(IStringLocalizer<HomeController> localizer, IHtmlLocalizer<HomeController> htmlLocalizer)
        {
            _localizer = localizer;
            _htmlLocalizer = htmlLocalizer;
        }

        public string Hello() => _localizer["HelloWorld"];

        public IActionResult Index()
        {
            ViewData["StringData"] = _localizer["HtmlTest"];
            ViewData["HtmlData"] = _htmlLocalizer["HtmlTest"];
            return View();
        }
    }
}
