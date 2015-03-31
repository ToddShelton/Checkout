using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Orleans;
using Orleans.Providers;

namespace Cloudrocket.Grains
{
    [StorageProvider(ProviderName = "AzureTableGrainState")]
    public class GrainCart : Grain<IGrainCartState>, IGrainCart
    {
        public override Task OnActivateAsync()
        {
            if (State.Cart == null) { State.Cart = new Cart(); }
            if (State.Cart.CartItems == null) { State.Cart.CartItems = new List<CartItem>(); }
            if (State.Cart.CartId == Guid.Empty) { State.Cart.CartId = this.GetPrimaryKey(); }

            return base.OnActivateAsync();
        }

        public async Task<Cart> AddCartItemAsync(CartItem cartItem)
        {
            try
            {
                if (cartItem != null)
                {
                    if (cartItem.CartItemId == Guid.Empty)
                    {
                        cartItem.CartItemId = Guid.NewGuid();
                    }

                    cartItem.CartId = State.Cart.CartId;

                    State.Cart.CartItems.Add(cartItem);

                    await UpdateCartAsync();
                }
                else
                {
                    throw new ArgumentException("Cart Item cannot be null.");
                }
            }
            catch (ArgumentException ex)
            {
                throw;
            }

            await UpdateCartAsync();

            return await Task.FromResult(State.Cart);
        }

        public async Task<Cart> RemoveCartItemAsync(CartItem cartItem)
        {
            State.Cart.CartItems.Remove(
                State.Cart.CartItems.Find(c => c.CartItemId == cartItem.CartItemId));

            await UpdateCartAsync();

            return await Task.FromResult(State.Cart);
        }

        public async Task<Cart> GetCartAsync()
        {
            await UpdateCartAsync();

            return await Task.FromResult(State.Cart);
        }

        public async Task<Cart> UpdateCartAsync()
        {
            foreach (var cartItem in State.Cart.CartItems)
            {
                cartItem.Subtotal = cartItem.Quantity * cartItem.Price;

                //cartItem.TaxCost =
                //   taxLookup.GetTax(cartItem.ProductTaxCode, cartItem.Subtotal).Result;

                cartItem.CartItemTotal = cartItem.Subtotal; // + cartItem.TaxCost
            }

            State.Cart.Subtotal = State.Cart.CartItems.Sum(item => item.Subtotal);
            State.Cart.TaxTotal = State.Cart.CartItems.Sum(item => item.TaxCost);
            State.Cart.Total = State.Cart.CartItems.Sum(item => item.CartItemTotal) + State.Cart.TaxTotal;

            await State.WriteStateAsync();

            return await Task.FromResult(State.Cart);
        }

        public async Task ClearStateAsync()
        {
            await State.ClearStateAsync();
        }
    }

    public interface IGrainCartState : IGrainState
    {
        Cart Cart { get; set; }
    }
}