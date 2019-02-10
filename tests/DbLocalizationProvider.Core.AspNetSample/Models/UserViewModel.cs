using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Core.AspNetSample.Resources;

namespace DbLocalizationProvider.Core.AspNetSample.Models
{
    [LocalizedModel]
    public class UserViewModel
    {
        public UserViewModel()
        {
            Address = new AddressViewModel();
        }

        public AddressViewModel Address { get; set; }

        [Display(Name = "User name:", Description = "This is description of UserName field")]
        [Required(ErrorMessage = "Name of the user is required!")]
        [WeirdCustom("Weird UserName attribute")]
        public string UserName { get; set; }

        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Password is kinda required :)")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Please use longer password than 5 symbols!!")]
        public string Password { get; set; }
    }
}
