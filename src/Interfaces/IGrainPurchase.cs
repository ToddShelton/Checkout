using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IGrainPurchase : IGrainWithGuidKey
    {
    }

    /// <summary>
    /// Aggregates all the components needed to process an Order into a Sale.
    /// </summary>
    [Serializable]
    public class Purchase
    {
        public Purchase()
        {
            this.Card = new Card();
            this.Cart = new Cart();
            this.Order = new Order();
            this.PaymentTransaction = new PaymentTransaction();

            this.PurchaseId = Guid.NewGuid();
        }

        public Purchase(Order order)
        {
            this.Card = new Card();
            this.Cart = new Cart();
            this.Order = order;
            this.PaymentTransaction = new PaymentTransaction(); 

            this.PurchaseId = Guid.NewGuid();
        }

        public Guid PurchaseId { get; set; }

        public PaymentTransaction PaymentTransaction { get; set; }

        public Card Card { get; set; }

        public Cart Cart { get; set; }

        public Order Order { get; set; }

        public decimal Subtotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public BaseAddress ShippingAddress { get; set; }
    }
}