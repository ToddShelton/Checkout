using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;

namespace Cloudrocket.GrainTests.UnitTests
{
    public class ServiceGrainBingLocationTests
    {
        [TestClass]
        public class GetGeocodeAsync
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Bing")]
            public async Task BingLocationCanGetGeocodeAsync()
            {
                // Arrange
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                // Act
                var testGrain = GrainFactory.GetGrain<IServiceGrainBingLocation>(Guid.NewGuid());
                var testGrainResult = await testGrain.GetGeocodeAsync();

                // Assert
                Assert.IsNotNull(testGrainResult);
                Assert.IsInstanceOfType(testGrainResult, typeof(GeoLocation));

                // Clean up
                GrainClient.Uninitialize();
            }

            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Bing")]
            public async Task BingLocationCanGetGeocodeAsyncFromStrings()
            {
                // Arrange
                string addressLine = "2400 NW 80th St. #163";
                string locality = "Seattle";
                string adminDistrict = "WA";
                string postalCode = "98117";
                string countryRegion = "US";

                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                // Act
                var testGrain = GrainFactory.GetGrain<IServiceGrainBingLocation>(Guid.NewGuid());
                var testGrainResult = await testGrain.GetGeocodeAsync(addressLine, locality, adminDistrict, postalCode, countryRegion);

                // Assert
                Assert.IsNotNull(testGrainResult);
                Assert.IsInstanceOfType(testGrainResult, typeof(GeoLocation));
                Assert.IsNotNull(testGrainResult.latitude);
                Assert.IsNotNull(testGrainResult.longitude);

                // Clean up
                GrainClient.Uninitialize();
            }

            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Bing")]
            public async Task BingLocationCanGetGeocodeAsyncFromGeoLocation()
            {
                // Arrange
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                var geoLocation = new GeoLocation()
                {
                    addressLine = "2400 NW 80th St. #163",
                    locality = "Seattle",
                    adminDistrict = "WA",
                    postalCode = "98117",
                    countryRegion = "US",
                };

                // Act
                var testGrain = GrainFactory.GetGrain<IServiceGrainBingLocation>(Guid.NewGuid());
                var testGrainResult = await testGrain.GetGeocodeAsync(geoLocation);

                // Assert
                Assert.IsNotNull(testGrainResult);
                Assert.IsInstanceOfType(testGrainResult, typeof(GeoLocation));
                Assert.IsNotNull(testGrainResult.latitude);
                Assert.IsNotNull(testGrainResult.longitude);

                // Clean up
                GrainClient.Uninitialize();
            }
        }
    }
}