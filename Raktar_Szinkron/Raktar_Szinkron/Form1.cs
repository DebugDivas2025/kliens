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
        private HotcakesApi api;

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
            HotcakesApi api = new HotcakesApi();

            var product = await api.GetProductBySkuAsync("kaspo052");

            if (product != null)
            {
                MessageBox.Show($"Termék: {product.ProductName}\nSKU: {product.Sku}");
            }
            else
            {
                MessageBox.Show("A termék nem található.");
            }
        }
    }
}
