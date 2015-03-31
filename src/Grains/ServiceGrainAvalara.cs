using System;
using System.Configuration;
using System.Threading.Tasks;
using Cloudrocket.AvaTax;
using Cloudrocket.Interfaces;
using Orleans;
using Orleans.Providers;

namespace Cloudrocket.Grains
{
    [StorageProvider(ProviderName = "AzureTableGrainState")]
    public class ServiceGrainAvalara : Grain<IServiceGrainAvalaraState>, IServiceGrainAvalara
    {
        private string _accountNumber = ConfigurationManager.AppSettings["AvaTax:AccountNumber(Dev)"];
        private string _companyCode = ConfigurationManager.AppSettings["AvaTax:CompanyCode(Dev)"];
        private string _licenseKey = ConfigurationManager.AppSettings["AvaTax:LicenseKey(Dev)"];
        private string _serviceUrl = ConfigurationManager.AppSettings["AvaTax:ServiceUrl(Dev)"];

        private AvaTax.LocationAddress _locationAddress = new AvaTax.LocationAddress();
        private AvaTax.GeoTaxResult _geoTaxResult;
        private AvaTax.GetTaxRequest _getTaxRequest;
        private AvaTax.GetTaxResult _getTaxResult;

        private AvaTax.TaxSvc _taxSvc;

        public override Task OnActivateAsync()
        {
            // Initialize grain state

            this._taxSvc = new TaxSvc(_accountNumber, _licenseKey, _serviceUrl);
            this._getTaxRequest = new GetTaxRequest();
            this._getTaxResult = new GetTaxResult();

            return base.OnActivateAsync();
        }

        public async Task<CancelTaxResult> CancelTax(CancelTaxRequest cancelTaxRequest)
        {
            // http://developer.avalara.com/api-docs/designing-your-integration/canceltax
            // DocCode/CompanyCode is the recommended transaction identification method, DocID not recommended.

            return await _taxSvc.CancelTax(cancelTaxRequest);
        }

        public async Task<GetTaxResult> GetTax(GetTaxRequest taxRequest)
        {
            _getTaxResult = await _taxSvc.GetTax(taxRequest);

            State.GetTaxResult = _getTaxResult;
            await State.WriteStateAsync();

            return await Task.FromResult(_getTaxResult);
        }

        public async Task<GeoTaxResult> GetTaxEstimate(string ProductTaxCode, decimal amount, decimal latitude, decimal longitude)
        {
            _geoTaxResult = await _taxSvc.EstimateTax(latitude, longitude, amount);

            return await Task.FromResult(_geoTaxResult);
        }

        public async Task<bool> PingTax()
        {
            GeoTaxResult geoTaxResult = await _taxSvc.Ping();

            return geoTaxResult != null ? true : false;
        }

        public async Task<ValidateResult> ValidateAddress(AvaTax.LocationAddress address)
        {
            _locationAddress = address;
            AddressSvc addressSvc = new AddressSvc(_accountNumber, _licenseKey, _serviceUrl);

            ValidateResult validateResult = addressSvc.Validate(_locationAddress);

            return await Task.FromResult(validateResult);
        }

        public async Task ClearStateAsync()
        {
            await State.ClearStateAsync();
        }
    }

    public interface IServiceGrainAvalaraState : IGrainState
    {
        AvaTax.GetTaxRequest GetTaxRequest { get; set; }

        AvaTax.GetTaxResult GetTaxResult { get; set; }
    }
}