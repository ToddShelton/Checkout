using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using SendGrid;

namespace Cloudrocket.Interfaces
{
    public interface IServiceGrainSendGrid : IGrainWithGuidKey
    {
        Task SendEmail(EmailMessage messsage);

        Task SendTestEmail(string _subjectText = null, string _messageBodyText = null, string _messageBodyHtml = null);
    }

    [Serializable]
    public class EmailMessage
    {
        public EmailMessage()
        {
            From = new EmailAddress();
            To = new EmailAddress();
        }

        public EmailAddress From { get; set; }

        public EmailAddress To { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public string Html { get; set; }

        public bool EnableClickTracking { get; set; }

        public string EnableFooter { get; set; }

        public bool EnableOpenTracking { get; set; }

        public string EnableTempate { get; set; }

        public string EnableTemplateEngine { get; set; }

        public string EnableUnsubscribe { get; set; }
    }

    [Serializable]
    public class EmailAddress
    {
        public EmailAddress()
        {
        }

        public string Address { get; set; }

        public string DisplayName { get; set; }
    }
}