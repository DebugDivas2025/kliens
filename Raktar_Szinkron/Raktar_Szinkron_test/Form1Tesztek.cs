using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;
using Raktar_Szinkron;
using Raktar_Szinkron.Models;
using Raktar_Szinkron.Sevices;
using System.Reflection;
using Moq;

namespace Raktar_Szinkron.Tests
{
    [Collection("Form1 alapműveletek")]
    public class Form1Tesztek : IDisposable
    {
        private readonly Form1 _form;  // readonly deklaráció

        // 🔧 Teszt-API, ami mindig "sikeres" választ ad
        public class TesztHotcakesApiAlwaysSuccess : HotcakesApi
        {
            public new KeszletFrissitesEredmeny FrissitesKeszletre(string sku, int eladottDb)
            {
                return new KeszletFrissitesEredmeny
                {
                    Sikeres = true,
                    Ar = 1490m,
                    EredetiKeszlet = 20,
                    UjKeszlet = 20 - eladottDb
                };
            }

            public new Task<Product> GetProductBySkuAsync(string sku)
            {
                // Csak a megadott SKU-ra ad választ
                if (sku == "palantavetomag001")
                {
                    return Task.FromResult(new Product
                    {
                        Sku = "palantavetomag001",
                        ProductName = "Bazsalikom palánta",
                        SitePrice = 1490m,
                        QuantityOnHand = 20
                    });
                }

                return Task.FromResult<Product>(null); // minden másra nem talál
            }

            public new Task<List<Product>> GetAllProductsAsync()
            {
                return Task.FromResult(new List<Product>
                {
                    new Product { Sku = "palantavetomag001", ProductName = "Bazsalikom palánta" },
                    new Product { Sku = "sku2", ProductName = "Genovese bazsalikom" },
                    new Product { Sku = "sku3", ProductName = "Saláta levelű bazsalikom" },
                    new Product { Sku = "sku4", ProductName = "Thai bazsalikom" },
                    new Product { Sku = "sku5", ProductName = "Vörös bazsalikom" }
                });
            }
        }

        public Form1Tesztek()
        {
            _form = new Form1(); // Az _form inicializálása
            _form.Show();

            // 🔧 _api mező lecserélése a saját példányunkra
            var apiField = typeof(Form1).GetField("_api", BindingFlags.NonPublic | BindingFlags.Instance);
            apiField.SetValue(_form, new TesztHotcakesApiAlwaysSuccess());
        }

        public void Dispose()
        {
            _form?.Close();
            _form?.Dispose();
        }

       

        [Fact]
        public void TestDeleteButton_ClearsSalesRecords()
        {
            var saleRecordsField = typeof(Form1).GetField("saleRecords", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = saleRecordsField.GetValue(_form) as System.Collections.IList;

            list.Add(new SaleRecord
            {
                SKU = "sku",
                Quantity = 1,
                SaleDate = DateTime.Now
            });

            Invoke("UpdateSalesGrid");

            var grid = (DataGridView)_form.Controls["dgvSales"];
            Assert.True(grid.RowCount > 0);

            Invoke("btnDelete_Click", new object[] { _form, EventArgs.Empty });

            Assert.Empty(list);
            Assert.Equal(0, grid.RowCount);
        }

        [Fact]
        public void UpdateSalesGrid_ShouldDisplayRecords()
        {
            var record = new SaleRecord
            {
                SKU = "testsku",
                Quantity = 2,
                SaleDate = DateTime.Today,
                Price = 1000,
                OriginalQuantity = 10,
                UpdatedQuantity = 8,
                Szinkronizalva = false
            };

            // A privát saleRecords lista beállítása
            var saleRecordsField = typeof(Form1).GetField("saleRecords", BindingFlags.NonPublic | BindingFlags.Instance);
            saleRecordsField.SetValue(_form, new List<SaleRecord> { record });

            // Meghívjuk a metódust, hogy frissítse a gridet
            var updateMethod = typeof(Form1).GetMethod("UpdateSalesGrid", BindingFlags.NonPublic | BindingFlags.Instance);
            updateMethod.Invoke(_form, null);

            // Act + Assert: csak a valódi adat sorokat számoljuk
            int actualRowCount = _form.dgvSales.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow);
            Assert.Equal(1, actualRowCount);
        }

        [Fact]
        public void Form_Load_DoesNotThrow()
        {
            // Form1_Load metódus nem dobhat kivételt (alapértelmezés)
            var method = typeof(Form1).GetMethod("Form1_Load", BindingFlags.NonPublic | BindingFlags.Instance);
            var exception = Record.Exception(() => method.Invoke(_form, new object[] { null, EventArgs.Empty }));
            Assert.Null(exception);
        }



        private void Invoke(string methodName, object[] parameters = null)
        {
            var method = _form.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(_form, parameters);

            
        }
        [Fact]
        public void TestClearForm_ShouldResetAllFields()
        {
            // Arrange: Beállítjuk a form mezőit
            _form.textBoxName.Text = "Teszt név";
            _form.textBoxSKU.Text = "SKU123";
            _form.textBoxQuantity.Text = "10";
            _form.textBoxPrice.Text = "5000";
            _form.dateTimePicker1.Value = new DateTime(2025, 1, 1);

            // Act: Meghívjuk a ClearForm metódust
            typeof(Form1).GetMethod("ClearForm", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_form, null);

            // Assert: Ellenőrizzük, hogy a mezők ki lettek ürítve
            Assert.Equal(string.Empty, _form.textBoxName.Text);
            Assert.Equal(string.Empty, _form.textBoxSKU.Text);
            Assert.Equal(string.Empty, _form.textBoxQuantity.Text);
            Assert.Equal(string.Empty, _form.textBoxPrice.Text);
            Assert.True((DateTime.Now - _form.dateTimePicker1.Value).TotalSeconds < 5); // Ellenőrizzük, hogy az időpont frissült-e
        }


    }
}
