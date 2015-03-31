using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IServiceGrainAvalara : IGrainWithStringKey
    {
        Task<AvaTax.CancelTaxResult> CancelTax(AvaTax.CancelTaxRequest cancelTaxRequest);

        /// <summary>
        /// Estimates tax using latitude and longitude to establish nexus.
        /// </summary>
        /// <param name="productTaxCode">The product's tax lookup code.</param>
        /// <param name="amount">The amount on which the tax will be calculated.</param>
        /// <param name="latitude">The nexus location's latitude.</param>
        /// <param name="longitude">The nexus location's longitude.</param>
        /// <returns>Estimated tax as decimal.</returns>
        Task<AvaTax.GetTaxResult> GetTax(AvaTax.GetTaxRequest taxRequest);

        /// <summary>
        /// Pings the AvaTax service to ensure that it's available and running properly.
        /// </summary>
        /// <returns></returns>
        Task<AvaTax.GeoTaxResult> GetTaxEstimate(string productTaxCode, decimal amount, decimal latitude, decimal longitude);

        /// <summary>
        ///  Gets and commits a tax calculation.  Use for checkout-type transactions.
        /// </summary>
        /// <param name="taxRequest"></param>
        /// <returns></returns>
        Task<bool> PingTax();

        /// <summary>
        /// Checks to ensure a successful
        /// tax nexus lookup for the address.
        /// </summary>
        /// <param name="addressToValidate">The nexus address as an AvaTax.Address.</param>
        /// <returns>Success if validated.</returns>
        Task<AvaTax.ValidateResult> ValidateAddress(AvaTax.LocationAddress addressToValidate);
        /// <summary>
        /// Deletes grain state from the backing store. /// </summary>
        /// <returns></returns>
        Task ClearStateAsync();
 
    }

    public interface ITaxGrainState : IGrainState
    {
        AvaTax.GetTaxRequest GetTaxRequest { get; set; }

        AvaTax.GetTaxResult GetTaxResult { get; set; }
    }

    [Serializable]
    public class TaxRequest : AvaTax.GetTaxRequest
    {
        public TaxRequest()
        {
        }
    }

    [Serializable]
    public class TaxResult : Cloudrocket.AvaTax.GetTaxResult
    {
        public TaxResult()
        {
        }
    }
}