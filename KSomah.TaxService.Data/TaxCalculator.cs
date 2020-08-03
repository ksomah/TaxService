using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KSomah.TaxService.Data
{
    public class TaxCalculator
    {
        private readonly Order _order;
        private readonly string _apiUri;
        private readonly string _apiKey;

        public TaxCalculator(Order order, string apiUri, string apiKey)
        {
            _order = order;
            _apiUri = apiUri;
            _apiKey = apiKey;
        }

        /// <summary>
        /// Calculates Tax Rate By Zipcode
        /// </summary>
        /// <returns></returns>
        public async Task<double> GetTaxRateByZipcodeAsync()
        {
            var request = (HttpWebRequest)WebRequest.Create($"{_apiUri}rates/{_order.ToZip}");
            request.Method = "GET";
            request.Headers.Add("Authorization", "Token token=\"" + _apiKey + "\"");
            var response = await request.GetResponseAsync();

            string apiResponse = null;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                apiResponse = await reader.ReadToEndAsync();
            }
            var objResponse = JObject.Parse(apiResponse);
            double taxRate = (double)objResponse["rate"]["combined_rate"];

            return taxRate;
        }

        /// <summary>
        /// Calculates Tax For Order 
        /// </summary>
        /// <returns></returns>
        public async Task<Tax> GetTaxForOrderAsync()
        {
            var rates = new Tax();

            var orderRate = "from_country=" + Uri.EscapeDataString(_order.FromCountry)
                     + "&from_zip=" + Uri.EscapeDataString(_order.FromZip)
                     + "&from_state=" + Uri.EscapeDataString(_order.FromState)
                     + "&from_city=" + Uri.EscapeDataString(_order.FromCity)
                     + "&from_street=" + Uri.EscapeDataString(_order.FromStreet)
                     + "&to_country=" + Uri.EscapeDataString(_order.ToCountry)
                     + "&to_zip=" + Uri.EscapeDataString(_order.ToZip)
                     + "&to_state=" + Uri.EscapeDataString(_order.ToState)
                     + "&to_city=" + Uri.EscapeDataString(_order.ToCity)
                     + "&to_street=" + Uri.EscapeDataString(_order.ToStreet)
                     + "&amount=" + Uri.EscapeDataString(_order.Amount.ToString())
                     + "&shipping=" + Uri.EscapeDataString(_order.ShippingAmount.ToString());

            var request = (HttpWebRequest)WebRequest.Create($"{_apiUri}taxes");
            request.Method = "POST";

            byte[] data = Encoding.UTF8.GetBytes(orderRate);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            request.Headers.Add("Authorization", "Token token=\"" + _apiKey + "\"");
            WebResponse response = await request.GetResponseAsync();

            string apiResponse = null;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                apiResponse = await reader.ReadToEndAsync();
            }
            var jsonRate = JObject.Parse(apiResponse);
            var jarrayRate = jsonRate.Root["tax"];
            rates.TaxAmount = Convert.ToDouble(jarrayRate["amount_to_collect"]);
            rates.TaxRate = Convert.ToDouble(jarrayRate["rate"]);

            return rates;
        }
    }
}
