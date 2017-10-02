using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AspNetIdentity.WebApi.Infrastructure;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AspNetIdentity.WebApi.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await SendEmailAsync(message);
        }

        private async Task SendEmailAsync(IdentityMessage message)
        {
            var apiKey = Environment.GetEnvironmentVariable("sendemail");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("cheranga@gmail.com", "Cheranga Hatangala"),
                Subject = message.Subject,
                PlainTextContent = message.Body,
                HtmlContent = message.Body
            };
            msg.AddTo(new EmailAddress(message.Destination));
            var emailResponse = await client.SendEmailAsync(msg);
        }
    }
}