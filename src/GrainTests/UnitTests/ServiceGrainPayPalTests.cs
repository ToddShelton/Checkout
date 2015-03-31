using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cloudrocket.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;
using PayPal.Api;

namespace Cloudrocket.GrainTests.UnitTests
{
    public class ServiceGrainPayPalTests
    {
        [TestClass]
        public class GetPayPalApiContextAsync
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("PayPal")]
            public async Task PayPalCanGetApiContext()
            {
                //Arrange Create the grain
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                // Act
                var testGrain = GrainFactory.GetGrain<IServiceGrainPayPal>(Guid.NewGuid());
                var test = await testGrain.GetPayPalApiContextAsync("requestId");

                // Assert
                Assert.IsNotNull(test);
                Assert.IsInstanceOfType(test, typeof(PayPal.Api.APIContext));

                // Clean up
                GrainClient.Uninitialize();
            }
        }

        [TestClass]
        public class GetPayPalPaymentAsync
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("PayPal")]
            public async Task PayPalCanGetPaymentAsync()
            {
                //Arrange Create the cart and card.
                Interfaces.OrderItem orderItem = new Interfaces.OrderItem()
                {
                    OrderItemId = Guid.NewGuid(),
                    Description = "This is a wonderful product!",
                    Price = 0.50M,
                    ProductSKU = "WP-2",
                    Name = "WonderProduct No. 2",
                    Quantity = 5M,
                    ProductUrl = "www.cloudrocket.com/wonderful-product",
                };

                Cloudrocket.Interfaces.Order testOrder = new Cloudrocket.Interfaces.Order()
                {
                    OrderItems = new System.Collections.Generic.List<Interfaces.OrderItem> { orderItem },
                    OrderId = Guid.NewGuid(),
                    Subtotal = 2.50M,
                    Total = 2.50M,
                };

                Card testCard = new Card()
                {
                    FirstName = "Joe",
                    LastName = "Shopper,",
                    CardCvvCode = "874",
                    CardType = "Visa",
                    CardNumber = "4877274905927862",
                    ExpirationMonth = "11",
                    ExpirationYear = "2018",
                    BillingAddress = new Interfaces.BaseAddress()
                    {
                        AddressLine1 = "1234 Your Street",
                        City = "Seattle",
                        StateProvince = "WA",
                        ZipPostalCode = "98117",
                        Country = "us"
                    },
                };

                Purchase purchase = new Purchase()
                {
                    Card = testCard,
                };

                // Act
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                var testGrain = GrainFactory.GetGrain<IServiceGrainPayPal>(Guid.NewGuid());
                var requestId = Guid.NewGuid().ToString();
                PayPal.Api.Payment testPayment = new PayPal.Api.Payment();

                try
                {
                    testPayment = await testGrain.GetPayPalPaymentAsync(testOrder, testCard, requestId);
                }
                catch (Exception ex)
                {
                    throw;
                }

                // Assert
                Assert.IsNotNull(testPayment);
                Assert.IsInstanceOfType(testPayment, typeof(PayPal.Api.Payment));

                // Clean up
                // Delete state
                //await testGrain.ClearStateAsync(); //TODO: Put a test on ClearState.  
                GrainClient.Uninitialize();
            }
        }
    }
}