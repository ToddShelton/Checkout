using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Orleans;
using Orleans.Providers;
using SendGrid;
using SendGrid.SmtpApi;

namespace Cloudrocket.Grains
{
    [StorageProvider(ProviderName = "AzureTableGrainState")]
    public class ServiceGrainSendGrid : Grain<IServiceGrainSendGridState>, IServiceGrainSendGrid
    {
        private string _sendgridApiPassword = string.Empty;
        private string _sendgridApiUsername = string.Empty;
        private static Web _web;

        public override Task OnActivateAsync()
        {
            _sendgridApiUsername = "SendGridApiUserName";
            _sendgridApiPassword = "SendGridApiPassword";
            _web = new Web(new NetworkCredential(_sendgridApiUsername, _sendgridApiPassword));

           return base.OnActivateAsync();
        }

        public async Task SendEmail(EmailMessage message)
        {
            SendGridMessage sendGridMessage = new SendGridMessage() {
                From = new MailAddress(message.From.Address, message.From.DisplayName),
                To = new MailAddress[]
                {new MailAddress(message.To.Address, message.To.DisplayName)
                },
                Subject = message.Subject,
                Text = message.Text,
                Html = message.Html,
            };

            if (message.EnableClickTracking) { sendGridMessage.EnableClickTracking(true); };
            if (!string.IsNullOrEmpty(message.EnableFooter)) { sendGridMessage.EnableFooter(message.EnableFooter); };
            if (message.EnableOpenTracking) { sendGridMessage.EnableOpenTracking(); };
            if (!string.IsNullOrEmpty(message.EnableTempate)) { sendGridMessage.EnableTemplate(message.EnableTempate); };
            if (!string.IsNullOrEmpty(message.EnableTemplateEngine)) { sendGridMessage.EnableTemplateEngine(message.EnableTemplateEngine); };
            if (!string.IsNullOrEmpty(message.EnableUnsubscribe)) { sendGridMessage.EnableUnsubscribe(message.EnableUnsubscribe); };

            try
            {
                Task t = _web.DeliverAsync(sendGridMessage);
                await t;

                if (t.Exception != null)
                {
                    throw new OrleansException(t.Exception.Message, t.Exception);
                }
            }
            catch (OrleansException orleansException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ;
            }

            return;
        }

        public async Task SendTestEmail(string _subjectText = null, string _messageBodyText = null, string _messageBodyHtml = null)
        {
            SendGridMessage message = new SendGridMessage() {
                From = new MailAddress("todd.shelton@cloudrocket.com", "Todd Shelton"),
                Header = new Header(),
                Subject = _subjectText != null ? _subjectText : "Default subject message",
                Text = _messageBodyText != null ? _messageBodyText : "This is default message text, used when no other message text is supplied.",
                Html = _messageBodyHtml != null ? _messageBodyHtml : "<h1>Default Html</h1> <p>This is default message html.</p>",
                To = new MailAddress[]
                {
                    new MailAddress("todd-shelton@msn.com", "Todd Shelton (msn)")
                },
            };

            message.EnableClickTracking();
            message.EnableFooter();
            message.EnableOpenTracking();
            message.EnableSpamCheck();
            message.EnableTemplate("<p>some HTML</p>");
            message.EnableTemplateEngine("templateId");
            message.EnableUnsubscribe("you can unsubscribe");
            //m.SetCategories(new IEnumerable<string> { "category 1" });

            try
            {
                Task t = _web.DeliverAsync(message);
                await t;

                if (t.Exception != null)
                {
                    throw new OrleansException(t.Exception.Message, t.Exception);
                }
            }
            catch (OrleansException orleansEx)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

            return;
        }
    }

    public interface IServiceGrainSendGridState : IGrainState
    {
        EmailMessage EmailMessage { get; set; }
    }
}