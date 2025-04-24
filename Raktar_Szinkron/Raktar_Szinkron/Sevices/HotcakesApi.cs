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

namespace Raktar_Szinkron.Sevices
{
    public class HotcakesApi
    {
        private readonly HttpClient client;

        public HotcakesApi(string baseUrl, string apiKey, string apiSecret)
        {
            client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<Product> GetProductBySkuAsync(string sku)
        {
            var response = await client.GetAsync($"DesktopModules/Hotcakes/API/rest/v1/products?sku=kaspo002?key=1-764ad1a8-4c6f-4bcd-bdc5-9af0f76aec39");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<Product>(json);
            
        }

        public async Task UpdateInventoryAsync(string sku, int newQty)
        {
            var content = new StringContent(JsonConvert.SerializeObject(
                new { QuantityOnHand = newQty }), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/hotcakes/products/inventory/{sku}", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
