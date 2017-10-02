using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var emailMessage = new SendGridMessage();

            //emailMessage.AddTo("cheranga@gmail.com");
            //emailMessage.From = new EmailAddress("cheranga@gmail.com", "Cheranga Hatangala");
            //emailMessage.Subject = "Hi!";
            //emailMessage.PlainTextContent = "Hello World from SendGrid!";
            //emailMessage.HtmlContent = "Hello World from SendGrid!";

            //var emailClient = new SendGridClient("sendemail", requestHeaders:new Dictionary<string, string>
            //{
            //    {"api_key", "ch3rasendgrid" },
            //    {"api_user", "cheranga" }
            //});
            //var result = emailClient.SendEmailAsync(emailMessage).Result;


            //var apiKey = Environment.GetEnvironmentVariable("sendemail");
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("test@example.com", "Example User");
            //var subject = "Sending with SendGrid is Fun";
            //var to = new EmailAddress("test@example.com", "Example User");
            //var plainTextContent = "and easy to do anywhere, even with C#";
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = client.SendEmailAsync(msg).Result;


            var apiKey = Environment.GetEnvironmentVariable("sendemail");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("cheranga@gmail.com", "Cheranga Hatangala"),
                Subject = "Hi",
                PlainTextContent = "Hello Email!",
                HtmlContent = "Hello Email!"
            };
            msg.AddTo(new EmailAddress("cheranga@gmail.com", "Cheranga"));
            var response = client.SendEmailAsync(msg).Result;
        }
    }
}
