using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using WebSite.Resources;

namespace WebSite.Models
{
    public class ConfirmPartecipationViewModel
    {
        public ParticipantInputModel Input { get; set; } = new ParticipantInputModel();

        public bool ShowConfirmation { get; set; }

        public IStringLocalizer Localization { get; set; }
    }

    public class ParticipantInputModel
    {
        [Required(
            ErrorMessageResourceType = typeof(EventsResource),
            ErrorMessageResourceName = "FullNameRequired"
        )]
        public string FullName { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(EventsResource),
            ErrorMessageResourceName = "TelephoneRequired"
        )]
        public string Telephone { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }

}
