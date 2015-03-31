using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Orleans;

namespace Cloudrocket.Interfaces
{
    public interface IProductGrain : IGrain
    {
        Task<string> GetProduct(string productId);
    }

    [Serializable]
    public class Product 
    {
        public Guid ProductItemId { get; set; }

        public string Manufacturer { get; set; }

        public string ProductName { get; set; }

        public string ProductSKU { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ProductTaxCode { get; set; }

        public Uri ProductImage { get; set; }

        public Uri ProductUrl { get; set; }

        public Uri ProductVideo { get; set; }

        public string Name { get; set; }

        public decimal ShippingWeight { get; set; }
    }
}