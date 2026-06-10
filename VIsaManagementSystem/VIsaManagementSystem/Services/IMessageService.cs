
using VisaManagementSystem.Enums;
namespace VisaManagementSystem.Services
{
    public interface IMessageService
    {
        string Get(MessageCodes code, params object[] args);
        string Get(string messageCode, params object[] args);
        Task<string> GetAsync(string messageCode, params object[] args);
        void ClearCache();
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendSmsAsync(string phoneNumber, string message);
    }
}

