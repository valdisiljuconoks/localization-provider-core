using System.Threading.Tasks;

namespace DbLocalizationProvider.Core.AspNetSample.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
