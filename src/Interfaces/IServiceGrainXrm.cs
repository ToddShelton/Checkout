namespace Cloudrocket.Interfaces
{
    using Cloudrocket.Xrm;
    using Microsoft.Xrm.Sdk;

    public class XrmProduct : Xrm.Product
    {
        // Product attributes are read-only as they are administered from MsCrm.
        public XrmProduct()
        {
            base.Price = new Microsoft.Xrm.Sdk.Money();
        }

        public string ProductName
        {
            get { return base.Name; }
        }

        new public string ProductNumber
        {
            get { return base.ProductNumber; }
            set { base.ProductNumber = value; }
        }

        public string ProductDescription
        {
            get { return base.Description; }
        }

        public decimal ShippingWeight
        {
            get { return (decimal)base.StockWeight; }
        }

        public string StockNumber
        {
            get { return base.ProductNumber; }
        }

        new public decimal Price
        {
            get { return base.Price.Value; }
            set { base.Price.Value = value; }
        }

        public string Vendor { get { return base.VendorName; } }

        public decimal GetProductTax(Product product, BaseAddress address)
        {
            decimal tax = 0.00M;
            return tax;
        }
    }

}