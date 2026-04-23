using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using WebSite.Model;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<HomeController> _localizer;

        private readonly IConfiguration  configuration;
        private readonly MailSettings _mailsettings;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            _logger = logger;
            this.configuration = configuration;
            _mailsettings = configuration.GetSection("MailSettings").Get<MailSettings>(); 
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CicDetails()
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

        [HttpPost]
        public ActionResult SendEmail(string email, string request)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email is required." });
            }

            try
            {
                var utils = new Utils(_mailsettings);
                var mailHelper = utils._mailHelper;
                var toEmail = _mailsettings.SMTP.User;
                request = "Nuova richiesta utente da " + email + "  <br> <br> " + request;
                mailHelper.SendEmail(toEmail, "Nuova richiesta utente", request);
                return Json(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }       

        public IActionResult ChangeLanguage(string lang)
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