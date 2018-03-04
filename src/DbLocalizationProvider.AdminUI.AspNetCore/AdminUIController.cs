using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore
{
    public class AdminUIController : Controller
    {
        public string Index()
        {
            return "{\"test\":1}";
        }
    }
}
