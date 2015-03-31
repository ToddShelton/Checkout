using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IGrainPaymentTransaction : IGrainWithGuidKey
    {
        Task<Purchase> ChargeAsync(PaymentTransaction paymentTransaction);
    }

    [Serializable]
    public class PaymentTransaction
    {
        public PaymentTransaction()
        {
        }

        /// <summary
        /// The CartId returned from the payment provider.
        /// </summary>
        public string CartId { get; set; }

        /// <summary>
        /// The time the payment was created.
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// The payment return code.  For PayPal,
        /// "approved" means that the payment was
        /// successfully processed.
        /// </summary>
        public string TransactionResult { get; set; }

        /// <summary>
        /// The current state of the transaction. 
        /// </summary>
        public string TransactionStatus { get; set; }

        /// <summary>
        /// The payment provider's transaction id.
        /// </summary>
        public string TransactionTrace { get; set; }
    }
}