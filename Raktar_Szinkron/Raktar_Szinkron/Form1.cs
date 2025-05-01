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
using System.IO;
using System.Diagnostics;

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

        private async void buttonAdd_Click(object sender, EventArgs e)
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

            string sku = textBoxSKU.Text.Trim();

            // 🔽 Lekérjük az árat és eredeti készletet
            decimal price = 0;
            int originalQty = 0;

            var product = await _api.GetProductBySkuAsync(sku);
            if (product != null)
            {
                price = product.SitePrice;
                originalQty = product.QuantityOnHand ?? 0;
            }

            // 🔽 Eladás létrehozása
            SaleRecord record = new SaleRecord
            {
                SKU = sku,
                Quantity = quantity,
                SaleDate = dateTimePicker1.Value,
                Price = price,
                OriginalQuantity = originalQty,
                UpdatedQuantity = originalQty
            };

            saleRecords.Add(record);

            UpdateSalesGrid();  // frissítjük a gridet
            ClearForm();        // kiürítjük a mezőket
                                //    if (string.IsNullOrWhiteSpace(textBoxSKU.Text) ||
                                //string.IsNullOrWhiteSpace(textBoxName.Text) ||
                                //string.IsNullOrWhiteSpace(textBoxQuantity.Text))
                                //    {
                                //        MessageBox.Show("Kérlek töltsd ki a Termék nevét, SKU-t és Mennyiséget!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //        return;
                                //    }

            //    if (!int.TryParse(textBoxQuantity.Text, out int quantity) || quantity <= 0)
            //    {
            //        MessageBox.Show("Érvényes mennyiséget adj meg!", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }

            //    // Eladás létrehozása
            //    SaleRecord record = new SaleRecord
            //    {
            //        SKU = textBoxSKU.Text.Trim(),
            //        Quantity = quantity,
            //        SaleDate = dateTimePicker1.Value
            //        // Ha később képet is akarsz: azt is hozzáadjuk majd
            //    };

            //    saleRecords.Add(record);

            //    // Frissítsük a DataGridView-t
            //    UpdateSalesGrid();

            //    ClearForm();
        }
        private void UpdateSalesGrid()
        {
            dgvSales.DataSource = null;              // Először nullázzuk, hogy frissüljön
            dgvSales.DataSource = saleRecords;        // Újra betöltjük a lista tartalmát

            //// Oszlopok újracímkézése csak egyszer kéne (de most gyorsan itt is)
            dgvSales.Columns["SKU"].HeaderText = "SKU";
            dgvSales.Columns["Quantity"].HeaderText = "Mennyiség";
            dgvSales.Columns["SaleDate"].HeaderText = "Időpont";
            dgvSales.Columns["Price"].HeaderText = "Ár (Ft)";
            dgvSales.Columns["OriginalQuantity"].HeaderText = "Eredeti készlet";
            dgvSales.Columns["UpdatedQuantity"].HeaderText="Új készlet";
            dgvSales.Columns["Szinkronizalva"].HeaderText = "Szinkronizálva";
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

        private async void btnSync_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSales.Rows)
            {
                if (row.IsNewRow) continue;

                string sku = row.Cells["SKU"].Value?.ToString();
                int quantitySold = Convert.ToInt32(row.Cells["Quantity"].Value);

                // Új típus visszatérés (nem bool!)
                var eredmeny = _api.FrissitesKeszletre(sku, quantitySold);

                if (eredmeny.Sikeres)
                {
                    row.Cells["Szinkronizalva"].Value = true;

                    // Grid frissítése az eredmény alapján
                    row.Cells["Price"].Value = eredmeny.Ar.ToString("0.00");
                    row.Cells["OriginalQuantity"].Value = eredmeny.EredetiKeszlet;
                    row.Cells["UpdatedQuantity"].Value = eredmeny.UjKeszlet;

                    // Színezés: ha új készlet < 10 → piros
                    if (eredmeny.UjKeszlet < 10)
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    else
                        row.DefaultCellStyle.BackColor = Color.White;
                }
            }

            MessageBox.Show("Szinkronizálás befejezve.");


        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (dgvSales.Rows.Count == 0)
            {
                MessageBox.Show("Nincs adat az exportáláshoz.", "Figyelmeztetés", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV fájl (*.csv)|*.csv";
                sfd.FileName = "eladasok.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                        {
                            // Oszlopfejlécek
                            for (int i = 0; i < dgvSales.Columns.Count; i++)
                            {
                                sw.Write(dgvSales.Columns[i].HeaderText);
                                if (i < dgvSales.Columns.Count - 1)
                                    sw.Write(";");
                            }
                            sw.WriteLine();

                            // Sorok
                            foreach (DataGridViewRow row in dgvSales.Rows)
                            {
                                if (row.IsNewRow) continue;

                                for (int i = 0; i < dgvSales.Columns.Count; i++)
                                {
                                    var value = row.Cells[i].Value?.ToString()?.Replace(";", ","); // ne legyen ; az adatban
                                    sw.Write(value);

                                    if (i < dgvSales.Columns.Count - 1)
                                        sw.Write(";");
                                }
                                sw.WriteLine();
                            }
                        }

                        MessageBox.Show("Exportálás sikeres!", "Siker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        string logFile = "sales_log.csv";
                        bool fileExists = File.Exists(logFile);

                        using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
                        {
                            if (!fileExists)
                                sw.WriteLine("SKU;Quantity;SaleDate;Price");

                            foreach (var record in saleRecords)
                            {
                                sw.WriteLine($"{record.SKU};{record.Quantity};{record.SaleDate:yyyy-MM-dd};{record.Price}");
                            }
                        }
                        try
                        {
                            Process.Start(new ProcessStartInfo()
                            {
                                FileName = sfd.FileName,
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("A fájl exportálása sikeres, de nem sikerült megnyitni:\n" + ex.Message);
                        }
                        // Automatikus megnyitás
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = sfd.FileName,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hiba exportálás közben:\n" + ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnOpenSalesLog_Click(object sender, EventArgs e)
        {
            var logForm = new SalesLogForm();
            logForm.Show();
        }
    }
}
