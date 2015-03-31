using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cloudrocket.Interfaces;

namespace Cloudrocket.GrainTests.UnitTests
{
    public class UnitTestVariables
    {
        public static readonly string orleansClientConfiguration = "OrleansClientConfiguration.xml";

        public static readonly BaseAddress _baseAddress = new BaseAddress()
        {
            AddressLine1 = "2400 NW 80th #163",
            City = "Seattle",
            StateProvince = "WA",
            ZipPostalCode = "98117",
            Country = "US",
        };

        public static readonly Cart testCart = new Cart()
        {
            CartId = Guid.NewGuid(),
            CartItems = new List<CartItem> { testCartItem },
        };

        public static readonly CartItem testCartItem = new CartItem()
        {
            CartItemId = Guid.NewGuid(),
            Currency = "USD",
            Description = "Launches rockets, just not where you want them to go.",
            Name = "Acme Rocket Launcher",
            Price = 99.99M,
            ProductTaxCode = "Acme",
            ProductSKU = "ARL-1",
            Quantity = 1M,
            ProductUrl = "https://www.cloudrocket.com/catalog/acme/arl-1",
            ShippingWeight = 19.50M,
        };

        public static readonly string testEmailMessageBodyText =
                "Hello,\n\n" +
                "This is a test message from SendGrid. " +
                "We have sent this to you because you requested a test message be sent from your account.\n\n" +
                "This is a link to google.com: http://www.google.com\n" +
                "This is a link to apple.com: http://www.apple.com\n" +
                "This is a link to sendgrid.com: http://www.sendgrid.com\n\n" +
                "Thank you for reading this test message.\n\nLove,\nYour friends at Cloudrocket"
            ;

        public static readonly string testEmailMessageBodyHtml =
            "<table style=\"font-size: 16px; border: none; background-color: #eee; font-family: verdana, tahoma, sans-serif; color: #000;\">" +
            "<tr> " +
            "<td> " +
            "<h2 style=\"font-size: 16px; color: #c00;\">Hello,</h2> " +
            "<p>This is a test message from SendGrid.   " +
            " We have sent this to you because you requested a test message be sent from your account.</p> " +
            "<a href=\"http://www.google.com\" target=\"_blank\">This is a link to google.com</a> " +
            "<p> <a href=\"http://www.apple.com\" target=\"_blank\">This is a link to apple.com</a> " +
            "<p> <a href=\"http://www.sendgrid.com\" target=\"_blank\">This is a link to sendgrid.com</a> </p> " +
            "<p>Thank you for reading this test message.</p> " +
            "Love,<br/> Your friends at Cloudrocket</p> <p> <img src=\"https://www.cloudrocket.com/content/logos/cloudrocket-logo-IoT.png\" alt=\"SendGrid!\" /> " +
            "</td> " +
            "</tr> </table>"
            ;

        public static readonly EmailMessage testEmailMessage = new EmailMessage()
        {
            From = new EmailAddress()
            {
                Address = "todd.shelton@cloudrocket.com",
                DisplayName = "Todd Shelton (Cloudrocket)",
            },
            To = new EmailAddress() { Address = "todd.shelton@cloudrocket.com", DisplayName = "Todd Shelton (CR)" },
            Subject = "Test email using SendGrid ",
            Text = testEmailMessageBodyText,
            Html = testEmailMessageBodyHtml,
        };

        public UnitTestVariables()
        {
        }
    }
}