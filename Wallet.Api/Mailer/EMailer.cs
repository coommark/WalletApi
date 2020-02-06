using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Wallet.Api.Mailer
{
    public class EMailer : IEMailer
    {
        private IHostingEnvironment _environment;
        public EMailer(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public void SendTransactionNotification(string to, string name, string amount, string balance, string message)
        {
            try
            {
                Email.DefaultRenderer = new RazorRenderer();
                Email.DefaultSender = new SmtpSender(() => new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("platinumtvhosting@gmail.com", "Platinum1960$")

                });
                var path = Path.Combine(_environment.WebRootPath, "emails", "alert.cshtml");

                var email = Email
                    .From("platinumtvhosting@gmail.com")
                    .To(to)
                    .Subject("E-Wallet Transaction Alert")
                    .UsingTemplateFromFile(path, new { Name = name, Email = to, Amount = amount, Balance = balance, Message = message });

                email.Send();
            }
            catch (Exception e)
            {
                //Do what with this? Retry? For how long? Sending emails from a business app is a f**king NO!
            }
        }

       
    }
}
