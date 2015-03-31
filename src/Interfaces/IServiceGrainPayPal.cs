using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IServiceGrainPayPal : IGrainWithGuidKey
    {
        Task<PayPal.Api.APIContext> GetPayPalApiContextAsync(string requestId);

        Task<PayPal.Api.Payment> GetPayPalPaymentAsync(Order order, Card card, string requestId = null);

        Task ClearStateAsync();
    }

    [Serializable]
    public class PaymentRequest : PayPal.Api.Payment
    {
    }

    [Serializable]
    public class PaymentResponse : PayPal.Api.Payment
    {
    }
}