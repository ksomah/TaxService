using KSomah.TaxService.Data;
using NUnit.Framework;

namespace KSomah.TaxService.Test
{
    [TestFixture]
    public class Tests
    {
        string url = null;
        string key = null;
        Order order;

        [SetUp]
        public void Setup()
        {
            url = "https://api.taxjar.com/v2/";
            key = "5da2f821eee4035db4771edab942a4cc";
            order = new Order
            {
                FromCountry = "US",
                FromCity = "La Jolla",
                FromStreet = "9500 Gilman Drive",
                FromState = "CA",
                FromZip = "92093",
                ToCity = "Upper Darby",
                ToCountry = "US",
                ToState = "PA",
                ToStreet = "250 Beverly Blvd",
                ToZip = "19082",
                Amount = 250,
                ShippingAmount = 12
            };
        }

        [Test]
        public void WhenGettingTaxRateByZipcode()
        {
            TaxCalculator taxCal = new TaxCalculator(order, url, key);
            var rate = taxCal.GetTaxRateByZipcodeAsync();
            Assert.IsNotNull(rate);
        }

        [Test]
        public void WhenGettingTaxForOrder()
        {
            TaxCalculator taxCal = new TaxCalculator(order, url, key);
            var rates = taxCal.GetTaxForOrderAsync();
            Assert.IsNotNull(rates);
        }
    }
}