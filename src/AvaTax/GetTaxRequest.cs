namespace Cloudrocket.AvaTax
{
    using System;

    /// <summary>
    /// The DocType sets the behavior of the tax request.
    /// </summary>
    public enum DocType
    {
        SalesOrder,
        SalesInvoice,
        ReturnOrder,
        ReturnInvoice,
        PurchaseOrder,
        PurchaseInvoice,
        ReverseChargeOrder,
        ReverseChargeInvoice
    }

    public enum DetailLevel
    {
        Tax,
        Document,
        Line,
        Diagnostic
    }

    public enum SystemCustomerUsageType
    {
        L, // "Other",
        A, // "Federal government",
        B, // "State government",
        C, // "Tribe / Status Indian / Indian Band",
        D, // "Foreign diplomat",
        E, // "Charitable or benevolent organization",
        F, // "Regligious or educational organization",
        G, // "Resale",
        H, // "Commercial agricultural production",
        I, // "Industrial production / manufacturer",
        J, // "Direct pay permit",
        K, // "Direct Mail",
        N, // "Local Government",
        P, // "Commercial Aquaculture",
        Q, // "Commercial Fishery",
        R // "Non-resident"
    }

    [Serializable]
    public class GetTaxRequest
    {
        // Required for tax calculation
        /// <summary>
        /// Transaction date. Required.
        /// </summary>
        public string DocDate { get; set; }

        /// <summary>
        /// (Required) The client application customer reference code.
        /// <para/>This is required since it is the key to the
        /// <para/>Exemption Certificate Management Service in the Admin Console.
        /// </summary>
        public string CustomerCode { get; set; }

        public LocationAddress[] Addresses { get; set; }

        public Line[] Lines { get; set; }

        // Best Practice for tax calculation
        public string Client { get; set; }

        public string DocCode { get; set; }

        public DocType DocType { get; set; }
        /// <summary>
        /// <para/>The code that identifies the company in the AvaTax account
        /// <para/>in which the document should be posted. This code is declared 
        /// <para/> during the company setup in the AvaTax Admin Console. If no value 
        /// <para/>is passed, the document will be assigned to the default company.
        /// </summary>
        public string CompanyCode { get; set; }

        public bool Commit { get; set; }

        public DetailLevel DetailLevel { get; set; }

        // Use where appropriate to the situation
        public string CustomerUsageType { get; set; }

        public string ExemptionNo { get; set; }

        public decimal Discount { get; set; }

        public string BusinessIdentificationNo { get; set; }

        public TaxOverrideDef TaxOverride { get; set; }

        public string CurrencyCode { get; set; }

        // Optional
        public string PurchaseOrderNo { get; set; }

        public string PaymentDate { get; set; }

        public string PosLaneCode { get; set; }

        public string ReferenceCode { get; set; }
    }
}