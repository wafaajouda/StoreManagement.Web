using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // تأكدي من إضافة هذه المكتبة

namespace StoreManagement.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        // حقن الـ IConfiguration لتمكين قراءة ملفات الإعدادات والـ Secrets
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Send(string to, string subject, string body)
        {
            // قراءة الإعدادات من ملف secrets.json (أو appsettings.json)
            var smtpServer = _configuration["EmailSettings:Server"];
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var password = _configuration["EmailSettings:Password"];

            var message = new MailMessage();
            message.From = new MailAddress(senderEmail, "Product App");
            message.Subject = subject;
            message.Body = body;
            message.To.Add(new MailAddress(to));
            message.IsBodyHtml = false;

            try
            {
                var emailClient = new SmtpClient
                {
                    Host = smtpServer,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail, password)
                };

                await emailClient.SendMailAsync(message);
            }
            catch (Exception e)
            {
                // طباعة الخطأ في نافذة الـ Output أو الـ Console لنعرف لو فيه مشكلة
                System.Diagnostics.Debug.WriteLine($"Email sending failed: {e.Message}");
                throw; // إعادة رمي الاستثناء مؤقتاً أثناء التجربة لمعرفة مكان الخلل بدقة
            }
        }
    }
}