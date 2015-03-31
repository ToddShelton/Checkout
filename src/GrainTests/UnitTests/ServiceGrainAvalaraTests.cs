using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;

namespace Cloudrocket.GrainTests.UnitTests
{
    public class ServiceGrainAvalaraTests
    {
        private static readonly AvaTax.LocationAddress[] testAddresses =
            {
                new AvaTax.LocationAddress {
                AddressCode = "01",
                Line1 = "2400 NW 80th Suite 163",
                City = "Seattle",
                //County = "King",
                Region = "WA",
                PostalCode = "98117",
                Country = "USA",
                }
            };

        private static readonly AvaTax.Line[] testLines =
            {
                new AvaTax.Line{
                Qty = 29M,
                Amount = 12.34M,
                LineNo = "01",
                DestinationCode = testAddresses[0].AddressCode,
                OriginCode = testAddresses[0].AddressCode,
                //TaxCode = "",
                },
            };

        private static readonly AvaTax.GetTaxRequest taxRequest = new AvaTax.GetTaxRequest
        {
            // Required.
            CompanyCode = "CloudrocketLabs",
            CustomerCode = "SampleCustomer", // required
            DocDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd"),
            Addresses = testAddresses,
            Lines = testLines,

            // Optional but best practice
            Commit = true,
            DetailLevel = AvaTax.DetailLevel.Diagnostic,
            DocCode = Guid.NewGuid().ToString(),
            DocType = AvaTax.DocType.SalesOrder, // *Invoice (Sales, Purchase, Return) writes the doc and makes it cancellable.
        };

        [TestClass]
        public class GetTax
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Avalara")]
            public async Task AvalaraCanGetTax()
            {
                //Arrange

                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                // Act
                var testGrain = GrainFactory.GetGrain<IServiceGrainAvalara>("ServiceGrainAvalaraCanGetTaxTest");
                AvaTax.GetTaxResult testGrainResult = await testGrain.GetTax(taxRequest);

                // Assert
                Assert.IsNotNull(testGrainResult);
                //Assert.IsInstanceOfType(testGrainResult, typeof(decimal));

                // Clean up
                await testGrain.ClearStateAsync(); 
                GrainClient.Uninitialize();
            }
        }

        [TestClass]
        public class GetTaxEstimate
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Avalara")]
            public async Task AvalaraCanGetTaxEstimate()
            {
                // Arrange
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                // Act
                var testGrain = GrainFactory.GetGrain<IServiceGrainAvalara>("ServiceGrainAvalaraCanGetTaxEstimateTest");
                AvaTax.GeoTaxResult testGrainResult = await testGrain.GetTaxEstimate("TaxCode", 10.00M, 47.0000M, 147.000M);

                // Assert
                Assert.IsNotNull(testGrainResult);
                Assert.IsInstanceOfType(testGrainResult, typeof(AvaTax.GeoTaxResult));

                // Clean up
                GrainClient.Uninitialize();
            }
        }

        [TestClass]
        public class CancelTax
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Avalara")]
            public async Task AvalaraCanCancelTax()
            {
                // Arrange

                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                // Act

                var testGrain = GrainFactory.GetGrain<IServiceGrainAvalara>("ServiceGrainAvalaraCanCancelTaxTest");

                AvaTax.GetTaxRequest testTaxRequest = taxRequest;
                testTaxRequest.DocType = AvaTax.DocType.SalesInvoice; // So we have a doc to cancel.

                AvaTax.GetTaxResult testGetTaxResult = await testGrain.GetTax(taxRequest);

                AvaTax.CancelTaxRequest testCancelTaxRequest = new AvaTax.CancelTaxRequest()
                     {
                         // DocCode/CompanyCode recommended transaction ID method, DocId not recommended.
                         CancelCode = AvaTax.CancelCode.DocDeleted,
                         CompanyCode = taxRequest.CompanyCode, // Required unless using DocId.
                         DocCode = testGetTaxResult.DocCode, // Required unless using DocId.
                         DocType = taxRequest.DocType, // Required: SalesInvoice, ReturnInvoice, or PurchaseInvoice.
                     };

                AvaTax.CancelTaxResult testCancelTaxResult = await testGrain.CancelTax(testCancelTaxRequest);

                // Assert
                Assert.IsNotNull(testCancelTaxResult);
                Assert.AreEqual(testCancelTaxResult.ResultCode.ToString(), "Success");

                // Clean up
                await testGrain.ClearStateAsync();
                GrainClient.Uninitialize();
            }
        }

        [TestClass]
        public class ValidateAddress
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Avalara")]
            public async Task AvalaraCanValidateAddress()
            {
                //Arrange
                if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }

                // Act
                var testGrain = GrainFactory.GetGrain<IServiceGrainAvalara>("ServiceGrainAvalaraCanGetTaxEstimateTest");
                var testGrainResult = await testGrain.ValidateAddress(testAddresses[0]);

                // Assert
                Assert.IsNotNull(testGrainResult);
                Assert.AreEqual(testGrainResult.ResultCode.ToString(), "Success");

                // Clean up
                GrainClient.Uninitialize();
            }
        }

        [TestClass]
        public class PingTax
        {
            [TestMethod]
            [TestCategory("Service Grains"), TestCategory("Grains"), TestCategory("Avalara")]
            public async Task AvalaraCanPingTax()
            {
                if (!GrainClient.IsInitialized) { GrainClient.Initialize("OrleansClientConfiguration.xml"); }

                var testGrain = GrainFactory.GetGrain<IServiceGrainAvalara>("ServiceGrainAvalaraCanPingTest");
                var testGrainResult = await testGrain.PingTax();

                // Assert
                Assert.IsNotNull(testGrainResult);
                Assert.IsInstanceOfType(testGrainResult, typeof(bool));

                // Clean up
                GrainClient.Uninitialize();

                // Act
            }
        }
    }
}