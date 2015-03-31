using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IServiceGrainPayPalConfiguration : IGrainWithStringKey
    {
        Task<Dictionary<string, string>> GetPayPalConfigurationAsync();
    }
}