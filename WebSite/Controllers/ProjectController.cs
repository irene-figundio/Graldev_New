using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace WebSite.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult AR()
        {
            return View();
        }


        public ActionResult Ludirex()
        {
            return View();
        }

        public ActionResult Geordie()
        {
            return View();
        }

        public ActionResult Parcor()
        {
            return View();
        }


        public IActionResult ChangeLanguage(string lang, string view)
        {
            CultureInfo cultureInfo;
            if (lang == "IT")
            {
                cultureInfo = new CultureInfo("it-IT");
            }
            else
            {
                cultureInfo = new CultureInfo("en");

            }
            //CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            return RedirectToAction("Index", "Home");

        }

    }
}
