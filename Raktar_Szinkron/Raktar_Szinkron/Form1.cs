using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Raktar_Szinkron.Sevices;
using Raktar_Szinkron.Models;

namespace Raktar_Szinkron
{
    public partial class Form1 : Form
    {
        //private HotcakesApi api;
        private HotcakesApi _api = new HotcakesApi();
        private List<SaleRecord> saleRecords = new List<SaleRecord>();

        public Form1()
        {
            InitializeComponent();
            //api = new HotcakesApi(
            //    "http://rendfejl1010.northeurope.cloudapp.azure.com/",
            //    "1-764ad1a8-4c6f-4bcd-bdc5-9af0f76aec39",
            //    "1-764ad1a8-4c6f-4bcd-bdc5-9af0f76aec39");
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private async void buttonTestApi_Click(object sender, EventArgs e)
        {

            HotcakesApi api = new HotcakesApi();
            bool result = await api.UpdateInventoryAsync("kaspo002", 20);
            MessageBox.Show(result ? "Készlet frissítve!" : "Hiba a frissítés során.");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //HotcakesApi api = new HotcakesApi();

            //var product = await api.GetProductBySkuAsync("kaspo002");

            //if (product != null)
            //{
            //    MessageBox.Show($"Termék: {product.ProductName}\nSKU: {product.Sku}");
            //}
            //else
            //{
            //    MessageBox.Show("A termék nem található.");
            //}
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxSKU.Text) ||
        string.IsNullOrWhiteSpace(textBoxName.Text) ||
        string.IsNullOrWhiteSpace(textBoxQuantity.Text))
            {
                MessageBox.Show("Kérlek töltsd ki a Termék nevét, SKU-t és Mennyiséget!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBoxQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Érvényes mennyiséget adj meg!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Eladás létrehozása
            SaleRecord record = new SaleRecord
            {
                SKU = textBoxSKU.Text.Trim(),
                Quantity = quantity,
                SaleDate = dateTimePicker1.Value
                // Ha később képet is akarsz: azt is hozzáadjuk majd
            };

            saleRecords.Add(record);

            // Frissítsük a DataGridView-t
            UpdateSalesGrid();

            ClearForm();
        }
        private void UpdateSalesGrid()
        {
            dgvSales.DataSource = null;              // Először nullázzuk, hogy frissüljön
            dgvSales.DataSource = saleRecords;        // Újra betöltjük a lista tartalmát

            //// Oszlopok újracímkézése csak egyszer kéne (de most gyorsan itt is)
            dgvSales.Columns["SKU"].HeaderText = "SKU";
            dgvSales.Columns["Quantity"].HeaderText = "Mennyiség";
            dgvSales.Columns["SaleDate"].HeaderText = "Időpont";

            dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
        }
        private void ClearForm()
        {
            textBoxName.Clear();
            textBoxSKU.Clear();
            textBoxPrice.Clear();
            textBoxQuantity.Clear();
            dateTimePicker1.Value = DateTime.Now;
        }

        private async void buttonFindProduct_Click(object sender, EventArgs e)
        {
            string searchText = textBoxName.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Írj be keresési kifejezést!", "Figyelmeztetés");
                return;
            }

            var allProducts = await _api.GetAllProductsAsync();
            if (allProducts == null)
            {
                MessageBox.Show("Nem sikerült lekérni a termékeket.", "Hiba");
                return;
            }

            var filteredProducts = allProducts.Where(p =>
                p.ProductName != null && p.ProductName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();

            listBoxProducts.Items.Clear();
            foreach (var product in filteredProducts)
            {
                listBoxProducts.Items.Add(product);
            }

            listBoxProducts.DisplayMember = "ProductName"; // <- Fontos, hogy mit jelenítsen meg a ListBox!

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxProducts.SelectedItem is Product selectedProduct)
            {
                textBoxName.Text = selectedProduct.ProductName;
                textBoxSKU.Text = selectedProduct.Sku;
                textBoxPrice.Text = selectedProduct.SitePrice.ToString("0.00");
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {

        }
    }
}
