using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using NoticeBoard.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NoticeBoard.Services
{
    public class PersonalInfo 
    {
        [JsonInclude]
        public string Email { get; set; }
        [JsonInclude]
        public string Password { get; set; }
    }
    public class InfoRetriever 
    {
        private const string infoPath = "./././info.json";
        public async Task<PersonalInfo> GetInfo() 
        {
            var serializer = new CustomSerializer<PersonalInfo>();
            var info = await serializer.deSerialize(infoPath);
            return info;
        }
    }

    public class MyCustomEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Administration of NoticeBoard web site", "no-reply@domain.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);

                var infoRetriver = new InfoRetriever();
                var info = await infoRetriver.GetInfo();

                await client.AuthenticateAsync(info.Email, info.Password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
