using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;

namespace Cloudrocket.GrainTests.UnitTests
{
    public class ServiceGrainPayPalConfigurationTests
    {
        [TestClass]
        public class GetPayPalConfiguration
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("PayPal")]
            public async Task PayPalCanGetConfiguration()
            {
                //Arrange Create the grain
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                var testGrain = GrainFactory.GetGrain<IServiceGrainPayPalConfiguration>("PayPalConfiguration");

                // Act
                var testResult = await testGrain.GetPayPalConfigurationAsync();

                // Assert
                Assert.IsNotNull(testResult);
                Assert.IsInstanceOfType(testResult, typeof(Dictionary<string, string>));

                // Clean up
                // Delete state
                GrainClient.Uninitialize();
            }
        }
    }
}