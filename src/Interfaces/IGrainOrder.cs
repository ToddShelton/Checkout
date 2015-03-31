using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
//using Cloudrocket.AvaTax;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IGrainOrder : IGrainWithGuidKey
    {
        Task GetPaymentInfo(Cart cart);

        Task ApproveOrder();

        Task ProcessPayment();

        Task ConfirmOrder();

        Task LogOrderToCrm();
    }

    [Serializable]
    public class Order
    {
        public Order()
        {
            this.OrderId = Guid.NewGuid();
            this.OrderItems = new List<OrderItem>();
        }

        public Order(Cart cart)
        {
            this.OrderId = Guid.NewGuid();
            this.OrderItems = new List<OrderItem>();
            cart.OrderId = this.OrderId;

            foreach (var cartItem in cart.CartItems)
            {
                // Check if this Cart item is already in this Order.
                OrderItem orderItem = this.OrderItems
                      .Where(o => o.ProductSKU == cartItem.ProductSKU)
                      .FirstOrDefault();

                // If the Cart item is not in the Order, add it.
                if (orderItem == null)
                {
                    orderItem = new OrderItem
                    {
                        OrderId = this.OrderId,
                        OrderItemId = Guid.NewGuid(),
                        Quantity = cartItem.Quantity,
                        ProductSKU = cartItem.ProductSKU,
                        Description = cartItem.Description,
                        Name = cartItem.Name,
                        Price = cartItem.Price,
                        ProductTaxCode = cartItem.ProductTaxCode,
                        ProductUrl = cartItem.ProductUrl,
                        ShippingWeight = cartItem.ShippingWeight,
                    };

                    this.OrderItems.Add(orderItem);
                }
                else
                {
                    // If the Cart item is already in the Order, update the quantity.
                    orderItem.Quantity = cartItem.Quantity;
                }

                orderItem.GetOrderItemTotals();
            }
            GetOrderTotals();
        }

        public Guid OrderId { get; set; }

        public int OrderNumber { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Order Description")]
        public string OrderDescription { get; set; }

        public Uri OrderURL { get; set; }

        public decimal Subtotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public void GetOrderTotals()
        {
            foreach (var orderItem in OrderItems)
            {
                orderItem.GetOrderItemTotals();
            }

            Subtotal = OrderItems.Sum(o => o.Subtotal);
            ShippingTotal = OrderItems.Sum(o => o.ShippingCost);
            TaxTotal = OrderItems.Sum(o => o.TaxCost);
            Total = OrderItems.Sum(o => o.OrderItemTotal);
        }
    }

    [Serializable]
    public class OrderItem
    {
        public OrderItem()
        {
            this.TaxResult = new TaxResult();
        }

        public Guid OrderId { get; set; }

        public Guid OrderItemId { get; set; }

        public Guid CartItemId { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ProductTaxCode { get; set; }

        public string ProductSKU { get; set; }

        public string ProductUrl { get; set; }

        public string Name { get; set; }

        public decimal Quantity { get; set; }

        public decimal Subtotal { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal ShippingWeight { get; set; }

        public decimal TaxCost { get; set; }

        public TaxResult TaxResult { get; set; }

        public decimal OrderItemTotal { get; set; }

        public void GetOrderItemTotals()
        {
            Subtotal = Quantity * Price;
            OrderItemTotal = Subtotal + ShippingCost + TaxCost;
        }
    }
}