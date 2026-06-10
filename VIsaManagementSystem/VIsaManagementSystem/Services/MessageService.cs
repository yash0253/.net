using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;
using VisaManagementSystem.Enums;
using VisaManagementSystem.Services;
using VisaManagementSystem.Data;
using VisaManagementSystem.Models;

namespace VisaManagementSystem.Services
{
    public class MessageService : IMessageService
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private static readonly string CacheKey = "SystemMessagesCache";

        public MessageService(
            DbContextOptions<ApplicationDbContext> dbOptions,
            IMemoryCache cache,
            IConfiguration config)
        {
            _dbOptions = dbOptions;
            _cache = cache;
            _config = config;
        }

        private async Task<IDictionary<string, SystemMessage>> LoadMessagesAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IDictionary<string, SystemMessage> messages))
            {
                using var context = new ApplicationDbContext(_dbOptions);

                messages = await context.SystemMessages
                    .Where(m => m.IsActive)
                    .AsNoTracking()
                    .ToDictionaryAsync(m => m.MessageCode, m => m);

                _cache.Set(CacheKey, messages, TimeSpan.FromHours(12));
            }

            return messages;
        }

        public string Get(MessageCodes code, params object[] args)
            => Get(code.ToString(), args);

        public string Get(string messageCode, params object[] args)
        {
            var messages = LoadMessagesAsync().GetAwaiter().GetResult();

            if (!messages.TryGetValue(messageCode, out var message))
                return $"[{messageCode}] Message not found in database.";

            return args.Length > 0 ? string.Format(message.MessageText, args) : message.MessageText;
        }

        public async Task<string> GetAsync(string messageCode, params object[] args)
        {
            var messages = await LoadMessagesAsync();

            if (!messages.TryGetValue(messageCode, out var message))
                return $"[{messageCode}] Message not found in database.";

            return args.Length > 0 ? string.Format(message.MessageText, args) : message.MessageText;
        }

        public void ClearCache()
        {
            _cache.Remove(CacheKey);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_config["SMTP:Host"], int.Parse(_config["SMTP:Port"] ?? "587"))
            {
                Credentials = new NetworkCredential(_config["SMTP:Username"], _config["SMTP:Password"]),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["SMTP:From"] ?? "noreply@visasystem.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            // Integrate with SMS provider (Twilio, etc.)
            // This is optional for now
            await Task.CompletedTask;
        }
    }
}