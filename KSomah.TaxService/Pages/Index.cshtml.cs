using System.Threading.Tasks;
using KSomah.TaxService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace KSomah.TaxService.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Property to store tax rate obtained by zipcode
        public string RateByZip { get; set; }

        // Property to store tax rate calculated based on order amount
        public string OrderTaxRate { get; set; }

        // Property to store collection of input properties from the UI
        [BindProperty]
        public InputModel Input { get; set; }

        // Input Properties
        public class InputModel
        {
            public string Country { get; set; }
            public string Zip { get; set; }
            public string State { get; set; }
            public string City { get; set; }
            public string Street { get; set; }
            public double Amount { get; set; }
            public double ShippingAmount { get; set; }
        }

        // Contructor to initialize configuration value
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        /// <summary>
        /// Calculates Taxes when the form Post
        /// </summary>
        /// <returns></returns>
        public async Task OnPostAsync()
        {
            var apiUri = _configuration.GetValue<string>("TaxJar:Uri");
            var apiKey = _configuration.GetValue<string>("TaxJar:Key");
            Order order = new Order
            {
                FromCountry = "US",
                FromCity = "La Jolla",
                FromStreet = "9500 Gilman Drive",
                FromState = "CA",
                FromZip = "92093",
                ToCity = Input.City,
                ToCountry = Input.Country,
                ToState = Input.State,
                ToStreet = Input.State,
                ToZip = Input.Zip,
                Amount = Input.Amount,
                ShippingAmount = Input.ShippingAmount
            };

            TaxCalculator taxCalculator = new TaxCalculator(order, apiUri, apiKey);
            var rate = await taxCalculator.GetTaxRateByZipcodeAsync();
            var orderTax = await taxCalculator.GetTaxForOrderAsync();

            if (orderTax.TaxAmount == 0)
            {
                RateByZip = $"Tax rate in your area is {rate * 100}% ";
                OrderTaxRate = $"Tax on your order is ${rate * Input.Amount}";
            }
            else
            {
                RateByZip = $"Tax rate in your area is {orderTax.TaxRate}% ";
                OrderTaxRate = $"Tax on your order is ${orderTax.TaxAmount}";
            }

        }
    }
}
