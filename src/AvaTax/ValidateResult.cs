namespace Cloudrocket.AvaTax
{
    using System;

    // Request for address/validate is parsed into the URI query parameters.
    [Serializable]
    public class ValidateResult
    {
        public LocationAddress Address { get; set; }

        public SeverityLevel ResultCode { get; set; }

        public Message[] Messages { get; set; }
    }
}
