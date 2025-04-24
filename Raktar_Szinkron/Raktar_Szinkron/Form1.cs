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
            string baseUrl = "http://rendfejl1010.northeurope.cloudapp.azure.com:80/";
            string apiKey = "1-764ad1a8-4c6f-4bcd-bdc5-9af0f76aec39";
            string apiSecret = "1-764ad1a8-4c6f-4bcd-bdc5-9af0f76aec39";

            api = new HotcakesApi(baseUrl, apiKey, apiSecret);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private async void buttonTestApi_Click(object sender, EventArgs e)
        {
            try
            {
                string sku = "kaspo002";
                var product = await api.GetProductBySkuAsync(sku);

                if (product != null)
                {
                    MessageBox.Show($"Termék: {product.Sku}\nKészlet: {product.QuantityOnHand}");
                }
                else
                {
                    MessageBox.Show("Nem található ilyen SKU.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt: " + ex.Message);
            }
        }
    }
}
