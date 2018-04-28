using Microsoft.AspNetCore.Mvc;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Models
{
    public class ServiceOperationResult
    {
        private ServiceOperationResult(string status)
        {
            Status = status;
        }

        public string Status { get; set; }

        public static JsonResult Ok => new JsonResult(new ServiceOperationResult("Ok"));
    }
}
