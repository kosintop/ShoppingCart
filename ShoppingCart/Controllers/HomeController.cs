using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ToggleLanguage()
        {
            var cookieValue = Request.Cookies.FirstOrDefault(x => x.Key == CookieRequestCultureProvider.DefaultCookieName).Value;
            var culture = CookieRequestCultureProvider.ParseCookieValue(cookieValue);
            var cultureString = "";

            if (culture!=null) cultureString = culture.Cultures.FirstOrDefault().Value;

            if (cultureString == "th-TH")
            {
               Response.Cookies.Append(
                   CookieRequestCultureProvider.DefaultCookieName,
                   CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US"))
               );
            }
            else
            {
               Response.Cookies.Append(
                   CookieRequestCultureProvider.DefaultCookieName,
                   CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("th-TH"))
               );
            }

            return RedirectToAction("Index","Home");
        }
    }
}
