using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Orleans;

namespace Cloudrocket.Grains
{
    public class ServiceGrainPayPalConfiguration : Grain, IServiceGrainPayPalConfiguration
    {
        public async Task<Dictionary<string, string>> GetPayPalConfigurationAsync()
        {
            // Get the PayPal configuration information from app.config.
            Dictionary<string, string> _payPalConfiguration = 
                PayPal.Api.ConfigManager.Instance.GetProperties();

            return await Task.FromResult(_payPalConfiguration);
        }
    }
}