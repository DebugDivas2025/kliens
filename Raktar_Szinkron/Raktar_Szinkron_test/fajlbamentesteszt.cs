using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Raktar_Szinkron;
using Raktar_Szinkron.Models;
using Raktar_Szinkron.Sevices;

namespace Raktar_Szinkron.Tests
{
    public class Form1Tests
    {
        private Form1 _form;

        public Form1Tests()
        {
            // Inicializáljuk a Form1-et
            _form = new Form1();

            var mockApi = new Mock<HotcakesApi>();
            mockApi.Setup(api => api.GetAllProductsAsync())
                   .Returns(Task.FromResult(new Product[]
                   {
           new Product { Sku = "palantavetomag001", ProductName = "Bazsalikom palánta", SitePrice = 1490m },
           new Product { Sku = "sku2", ProductName = "Genovese bazsalikom", SitePrice = 1290m }
                   }));

            _form._api = mockApi.Object;
        }

        [Fact]
        public void TestCsvExport_CreatesFileAndContainsCorrectData()
        {
            // Arrange: Előkészítjük a formot és a szükséges adatokat
            var saleRecordsField = typeof(Form1).GetField("saleRecords", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var saleRecords = new List<SaleRecord>
            {
                new SaleRecord { SKU = "palantavetomag001", Quantity = 2, Price = 1490m, SaleDate = DateTime.Today },
                new SaleRecord { SKU = "sku2", Quantity = 1, Price = 1290m, SaleDate = DateTime.Today }
            };
            saleRecordsField.SetValue(_form, saleRecords);

            // A fájl teljes elérési útja
            string exportedFilePath = Path.Combine(Environment.CurrentDirectory, "eladasok.csv");

            // Act: Meghívjuk az exportálás funkciót
            _form.btnExportCsv_Click(null, EventArgs.Empty);

            // Biztosítjuk, hogy az exportálás befejeződjön és várunk egy kicsit
            Task.Delay(2000).Wait(); // Várakozás az aszinkron műveletek befejezésére

            // Assert: Ellenőrizzük, hogy a fájl létrejött
            Assert.True(File.Exists(exportedFilePath), "CSV fájl nem jött létre!");

            // Assert: Ellenőrizzük a fájl tartalmát
            var fileContent = File.ReadAllText(exportedFilePath);
            Assert.Contains("SKU;Quantity;SaleDate;Price", fileContent); // Ellenőrizzük az oszlopfejléceket
            Assert.Contains("palantavetomag001;2;2025-05-03;1490.00", fileContent); // Ellenőrizzük az adatokat
            Assert.Contains("sku2;1;2025-05-03;1290.00", fileContent); // Ellenőrizzük a második rekordot

            // Tisztítás: Töröljük a fájlt a teszt után
            File.Delete(exportedFilePath);
        }
    }
}
