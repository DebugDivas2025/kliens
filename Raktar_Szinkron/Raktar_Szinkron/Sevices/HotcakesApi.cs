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
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Raktar_Szinkron.Sevices
{
    public class HotcakesApi
    {
        private readonly Api _sdkApi;
        private readonly HttpClient _client;
        private readonly string _baseUrl = "http://rendfejl1010.northeurope.cloudapp.azure.com/DesktopModules/Hotcakes/API/rest/v1/";
        private readonly string _apiKey = "1-5dd6e74c-0fdb-48f4-a47d-1e3a84811bd5";


        public HotcakesApi()
        {
            string url = "http://rendfejl1010.northeurope.cloudapp.azure.com";
            string apiKey = "1-5dd6e74c-0fdb-48f4-a47d-1e3a84811bd5";
            _sdkApi = new Api(url, apiKey);
            _client = new HttpClient();
            
        }

        public async Task<Product[]> GetAllProductsAsync()
        {
            string url = $"{_baseUrl}products?key={_apiKey}";

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<ProductResponse>(json);

                return responseData?.Content?.Products?.ToArray(); // <- Így jó!
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
                MessageBox.Show(json);
                var responseData = JsonConvert.DeserializeObject<ProductResponse>(json);

                return responseData?.Content?.Products?.FirstOrDefault();
            }

            return null;
        }


        public bool FrissitesKeszletre(string sku, int mennyiseg)
        {
            // 1. Termék keresése SKU alapján
            var productResponse = _sdkApi.ProductsFindBySku(sku);

            if (productResponse == null || productResponse.Content == null)
            {
                MessageBox.Show($"Nem található termék ezzel az SKU-val: {sku}");
                return false;
            }

            var product = productResponse.Content;

            // 2. Inventory lekérése
            var inventoryResponse = _sdkApi.ProductInventoryFindForProduct(product.Bvin);

            if (inventoryResponse == null || inventoryResponse.Content == null || inventoryResponse.Content.Count == 0)
            {
                MessageBox.Show($"Nem található készletinformáció a termékhez: {sku}");
                return false;
            }

            var inventory = inventoryResponse.Content.First();

            // 3. Készlet csökkentése
            int jelenlegiKeszlet = inventory.QuantityOnHand;
            int ujKeszlet = Math.Max(0, jelenlegiKeszlet - mennyiseg);
            inventory.QuantityOnHand = ujKeszlet;

            // 4. Frissítés küldése
            var updateResponse = _sdkApi.ProductInventoryUpdate(inventory);

            if (updateResponse.Errors != null && updateResponse.Errors.Count > 0)
            {
                MessageBox.Show($"Hiba a frissítéskor: {string.Join(", ", updateResponse.Errors.Select(e => e.Description))}");
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateInventoryAsync(string bvin, int newQuantity)
        {
            //string url = $"{_baseUrl}products/{bvin}/inventory?key={_apiKey}";

            //var payload = new
            //{
            //    QuantityOnHand = newQuantity
            //};

            //var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            //HttpResponseMessage response = await _client.PostAsync(url, content);
            //return response.IsSuccessStatusCode;
            string url = $"{_baseUrl}products/{bvin}/inventory?key={_apiKey}";

            var payload = new
            {
                QuantityOnHand = newQuantity
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show($"API válasz:\n{responseBody}", "Frissítés hiba");
            }

            return response.IsSuccessStatusCode;
        }


    }
}
