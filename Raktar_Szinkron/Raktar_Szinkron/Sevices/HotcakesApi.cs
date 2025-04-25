using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Raktar_Szinkron.Models;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Raktar_Szinkron.Sevices
{
    public class HotcakesApi
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "http://rendfejl1010.northeurope.cloudapp.azure.com/DesktopModules/Hotcakes/API/rest/v1/";
        private readonly string _apiKey = "1-764ad1a8-4c6f-4bcd-bdc5-9af0f76aec39";


        public HotcakesApi()
        {
            _client = new HttpClient();
            
        }

        public async Task<Product[]> GetAllProductsAsync()
        {
            string url = $"{_baseUrl}products?key={_apiKey}";

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Product[]>(json);
            }

            return null;
        }

        public async Task<Product> GetProductBySkuAsync(string sku)
        {
            string url = $"{_baseUrl}products?key={_apiKey}&sku={sku}";

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<ProductResponse>(json);

                return responseData?.Content?.Products?.FirstOrDefault();
            }

            return null;
        }

        public async Task<bool> UpdateInventoryAsync(string sku, int newQuantity)
        {
            string url = $"{_baseUrl}products/inventory/{sku}?key={_apiKey}";
            var payload = new
            {
                Quantity = newQuantity
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync(url, content);
            return response.IsSuccessStatusCode;
        }


    }
}
