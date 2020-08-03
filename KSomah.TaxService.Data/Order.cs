/*
 * Order class conataing properties to hold Tax calcalation data
 */

namespace KSomah.TaxService.Data
{
    public class Order
    {
        public string FromCountry { get; set; }
        public string FromZip { get; set; }
        public string FromState { get; set; }
        public string FromCity { get; set; }
        public string FromStreet { get; set; }
        public string ToCountry { get; set; }
        public string ToZip { get; set; }
        public string ToState { get; set; }
        public string ToCity { get; set; }
        public string ToStreet { get; set; }
        public double Amount { get; set; }
        public double ShippingAmount { get; set; }
    }
}
