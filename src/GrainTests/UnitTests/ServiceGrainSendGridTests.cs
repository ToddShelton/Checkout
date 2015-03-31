using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;
using SendGrid;
using SendGrid.SmtpApi;

namespace Cloudrocket.GrainTests.UnitTests
{
    public class SendGridGrainTests
    {
        private static string _sendgridApiUsername = "SendGridUserName";
        private static string _sendgridApiPassword = "SendGridApiPassword";
        private static SendGrid.Web _web = new SendGrid.Web(new NetworkCredential(_sendgridApiUsername, _sendgridApiPassword));

        private static EmailMessage _testEmailMessage = UnitTestVariables.testEmailMessage;

        private static SendGridMessage _testSendGridMessage = new SendGridMessage()
        {
            From = new MailAddress(_testEmailMessage.From.Address, _testEmailMessage.From.DisplayName),
            To = new MailAddress[]
                {new MailAddress(_testEmailMessage.To.Address, _testEmailMessage.To.DisplayName)
                },
            Subject = _testEmailMessage.Subject,
            Text = _testEmailMessage.Text != null ? _testEmailMessage.Text : string.Empty,
            Html = _testEmailMessage.Html,
        };

        [TestClass]
        public class SendTestEmailDirect
        {
            [TestCategory("Service Direct"), TestCategory("SendGrid")]
            [TestMethod]
            public async Task EmailCanSendTestDirect()
            {
                // Arrange

                // Act

                // Send direct, bypass grain
                var transport = new SendGrid.Web(new NetworkCredential(_sendgridApiUsername, _sendgridApiPassword));

                try
                {
                    _testSendGridMessage.Subject = "(CanSendTestEmailDirect) Test message";

                    Task t = transport.DeliverAsync(_testSendGridMessage);
                    await t;

                    if (t.Exception != null)
                    {
                        throw new ApplicationException(t.Exception.Message, t.Exception);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                return;
            }
        }

        [TestClass]
        public class SendTestEmailUsingGrain
        {
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("SendGrid")]
            [TestMethod]
            public async Task EmailCanSendTestUsingGrain()
            {
                // Arrange
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                IServiceGrainSendGrid sendGridGrain = GrainFactory.GetGrain<IServiceGrainSendGrid>(Guid.NewGuid());

                // Act
                try
                {
                    _testEmailMessage.Subject = "(CanSendTestEmailUsingGrain) Test message";

                    Task t = sendGridGrain.SendEmail(_testEmailMessage);
                    await t;

                    if (t.Exception != null)
                    {
                        throw new ApplicationException(t.Exception.Message, t.Exception);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                // Clean up
                // Delete state
                GrainClient.Uninitialize();
            }
        }
    }
}