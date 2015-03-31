using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudrocket.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans;

namespace Cloudrocket.GrainTests.UnitTests
{
    [TestClass]
    public class CartGrainTests
    {
        private static CartItem _testCartItem = UnitTestVariables.testCartItem;

        // Arrange
        private CartItem _cartItem = new CartItem()
        {
            CartItemId = Guid.NewGuid(),
            Currency = "USD",
            Description = "Test cart item description",
            Name = "Cart item name",
            Price = 1.23M,
            ProductSKU = "Product SKU",
            ProductTaxCode = "DefaultTaxCode",
            Quantity = 2M,
            ShippingCost = 0M,
            TaxCost = 0.12M,
        };

        [TestMethod]
        [TestCategory("Grains")]
        public async Task CartCanAddCartItemAsync()
        {
            // Arrange
            if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }
            IGrainCart cartGrain = GrainFactory.GetGrain<IGrainCart>(Guid.NewGuid());
            Cart cart = new Cart();

            // Act
            try
            {
                cart = await cartGrain.AddCartItemAsync(_cartItem);
            }
            catch (Exception ex)
            {
                throw;
            }

            // Assert
            Assert.IsNotNull(cart);
            Assert.IsInstanceOfType(cart, typeof(Cart));
            Assert.IsNotNull(cart.CartItems);
            Assert.AreEqual(cart.CartItems.Count, 1);

            // Clean up
            await cartGrain.ClearStateAsync();
            GrainClient.Uninitialize();
        }

        [TestMethod]
        [TestCategory("Grains")]
        public async Task CartCanGetCartAsync()
        {
            // Arrange
            if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }
            IGrainCart cartGrain = GrainFactory.GetGrain<IGrainCart>(Guid.NewGuid());
            Cart cart = new Cart();

            // Act
            cart = await cartGrain.GetCartAsync();

            // Assert
            Assert.IsNotNull(cart);
            Assert.IsInstanceOfType(cart, typeof(Cart));
            Assert.IsNotNull(cart.CartItems);

            // Clean up
            await cartGrain.ClearStateAsync();
            GrainClient.Uninitialize();
        }

        [TestMethod]
        [TestCategory("Grains")]
        public async Task CartCanRemoveCartItemAsync()
        {
            // Arrange
            if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }
            IGrainCart cartGrain = GrainFactory.GetGrain<IGrainCart>(Guid.NewGuid());
            Cart cart = new Cart(); // do we have CartItems?

            // Act
            cart = await cartGrain.AddCartItemAsync(_cartItem);

            cart = await cartGrain.RemoveCartItemAsync(_cartItem);

            var t = cart.CartItems.Find(i => i.CartItemId == _cartItem.CartItemId);

            // Assert
            Assert.IsNotNull(cart);
            Assert.IsInstanceOfType(cart, typeof(Cart));
            Assert.IsNotNull(cart.CartItems);
            Assert.AreEqual(cart.CartItems.Count, 0);

            // Clean up
            await cartGrain.ClearStateAsync();
            GrainClient.Uninitialize();
        }

        [TestMethod]
        [TestCategory("Grains")]
        public async Task CartCanUpdateCartAsync()
        {
            // Arrange
            if (!GrainClient.IsInitialized) { GrainClient.Initialize(); }
            IGrainCart cartGrain = GrainFactory.GetGrain<IGrainCart>(Guid.NewGuid());
            Cart cart = new Cart();

            // Act
            cart = await cartGrain.AddCartItemAsync(_cartItem);
            await cartGrain.UpdateCartAsync();
            cart = await cartGrain.GetCartAsync();

            // Assert
            Assert.IsNotNull(cart);
            Assert.IsInstanceOfType(cart, typeof(Cart));
            Assert.IsNotNull(cart.CartItems);
            Assert.AreEqual(cart.Total, 2.58M);

            // Clean up
            await cartGrain.ClearStateAsync();
            GrainClient.Uninitialize();
        }
    }
}