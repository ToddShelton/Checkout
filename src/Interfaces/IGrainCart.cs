using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IGrainCart : IGrainWithGuidKey
    {
        Task<Cart> AddCartItemAsync(CartItem cartItem);

        Task<Cart> RemoveCartItemAsync(CartItem cartItem);

        Task<Cart> GetCartAsync();

        Task<Cart> UpdateCartAsync();

        Task ClearStateAsync();
    }

    [Serializable]
    public class Cart
    {
        public Cart()
        {
            CartId = Guid.NewGuid();
            CartItems = new List<CartItem>();
        }

        public Guid CartId { get; set; }

        public Guid OrderId { get; set; }

        public List<CartItem> CartItems { get; set; }

        public decimal Subtotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public BaseAddress ShippingAddress { get; set; }

        public void GetCartTotals()
        {
            Subtotal = CartItems.Sum(o => o.Subtotal);
            ShippingTotal = CartItems.Sum(o => o.ShippingCost);
            TaxTotal = CartItems.Sum(o => o.TaxCost);
            Total = CartItems.Sum(o => o.CartItemTotal);
        }
    }

    [Serializable]
    public class CartItem
    {
        public CartItem()
        {
            this.TaxResult = new TaxResult();
        }

        public Guid CartId { get; set; }

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

        public decimal CartItemTotal { get; set; }

        public void GetCarttemTotals()
        {
            Subtotal = Quantity * Price;
            CartItemTotal = Subtotal + ShippingCost + TaxCost;
        }
    }
}