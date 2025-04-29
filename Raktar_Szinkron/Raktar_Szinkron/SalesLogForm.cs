using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Raktar_Szinkron
{
    public partial class SalesLogForm : Form
    {
        public SalesLogForm()
        {
            InitializeComponent();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string logFile = "sales_log.csv";
            if (!File.Exists(logFile))
            {
                MessageBox.Show("Nincs elérhető eladási napló.");
                return;
            }

            var fromDate = dtFrom.Value.Date;
            var toDate = dtTo.Value.Date;

            var lines = File.ReadAllLines(logFile).Skip(1); // fejléc kihagyása

            var query = lines
                .Select(line => line.Split(';'))
                .Select(parts => new
                {
                    SKU = parts[0],
                    Quantity = int.Parse(parts[1]),
                    SaleDate = DateTime.Parse(parts[2]),
                    Price = decimal.Parse(parts[3])
                })
                .Where(x => x.SaleDate >= fromDate && x.SaleDate <= toDate)
                .GroupBy(x => x.SKU)
                .Select(g => new
                {
                    SKU = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            dgvSalesLog.DataSource = query;
        }
    }
}
