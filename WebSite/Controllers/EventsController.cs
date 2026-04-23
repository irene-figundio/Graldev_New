using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.IO;
using System.Linq;
using WebSite.Models;
using WebSite.Resources;

public class EventsController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IStringLocalizer<EventsResource> _localizer;


    public EventsController(
        IWebHostEnvironment env,
        IStringLocalizer<EventsResource> factory)
    {
        _env = env;
        // punta ai file ConfirmPartecipationResource.*
    _localizer = factory;
    }

    // GET /Events/Confirm
    [HttpGet]
    public IActionResult Confirm(string culture, string email)
    {
        var model = new ConfirmPartecipationViewModel
        {
            Localization = _localizer,
             Input = new ParticipantInputModel
             {
                 Email = email ?? ""   // <-- IMPORTANTE
             }
        };

        if (!string.IsNullOrEmpty(email))
            model.Input.Email = email;

        var folder = Path.Combine(_env.WebRootPath, "data");
        var filePath = Path.Combine(folder, "Partecipants.csv");

        if (System.IO.File.Exists(filePath) && !string.IsNullOrWhiteSpace(email))
        {
            bool exists = System.IO.File.ReadAllLines(filePath)
                .Any(line => line.Contains($"\"{email}\"", StringComparison.OrdinalIgnoreCase));

            model.ShowConfirmation = exists;
        }

        return View("ConfirmPartecipation", model);
    }

    // POST /Events/Submit
    [HttpPost]
    public IActionResult Submit([FromForm] ConfirmPartecipationViewModel model)
    {
        model.Localization = _localizer;
        if (string.IsNullOrEmpty(model.Input.FullName) 
            || string.IsNullOrEmpty(model.Input.Telephone) 
            || string.IsNullOrEmpty(model.Input.Email))
        {
            return View("ConfirmPartecipation", model);
        }

        //if (!ModelState.IsValid)
        //{
        //    foreach (var entry in ModelState)
        //    {
        //        foreach (var error in entry.Value.Errors)
        //        {
        //            Console.WriteLine($"{entry.Key}: {error.ErrorMessage}");
        //        }
        //    }

        //    return View("ConfirmPartecipation", model);
        //}

        var folder = Path.Combine(_env.WebRootPath, "data");
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, "Partecipants.csv");


        var line = string.Join(",",
            $"\"{DateTime.Now:o}\"",
            $"\"{model.Input.FullName}\"",
            $"\"{model.Input.Telephone}\"",
            $"\"{model.Input.Company}\"",
            $"\"{model.Input.Email}\""
        );

        System.IO.File.AppendAllText(filePath, line + "\n");
        // redirect per mostrare la conferma
        return RedirectToAction("Confirm", new { email = model.Input.Email });
    }
}
