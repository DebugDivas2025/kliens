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
                MessageBox.Show("Az eladási napló fájl nem található.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var lines = File.ReadAllLines(logFile).Skip(1); // fejléc kihagyása

            var allRecords = lines
                .Select(line => line.Split(';'))
                .Select(parts => new
                {
                    SKU = parts[0],
                    Quantity = int.Parse(parts[1]),
                    SaleDate = DateTime.Parse(parts[2]),
                    Price = decimal.Parse(parts[3])
                });

            // dátumszűrés csak ha checkbox be van pipálva
            DateTime fromDate = dtFrom.Checked ? dtFrom.Value.Date : DateTime.MinValue;
            DateTime toDate = dtTo.Checked ? dtTo.Value.Date : DateTime.MaxValue;

            var query = allRecords
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
            //string logFile = "sales_log.csv";
            //if (!File.Exists(logFile))
            //{
            //    MessageBox.Show("Nincs elérhető eladási napló.");
            //    return;
            //}
            
            //var fromDate = dtFrom.Value.Date;
            //var toDate = dtTo.Value.Date;
            
            //var lines = File.ReadAllLines(logFile).Skip(1); // fejléc kihagyása

            //var query = lines
            //    .Select(line => line.Split(';'))
            //    .Select(parts => new
            //    {
            //        SKU = parts[0],
            //        Quantity = int.Parse(parts[1]),
            //        SaleDate = DateTime.Parse(parts[2]),
            //        Price = decimal.Parse(parts[3])
            //    })
            //    .Where(x => x.SaleDate >= fromDate && x.SaleDate <= toDate)
            //    .GroupBy(x => x.SKU)
            //    .Select(g => new
            //    {
            //        SKU = g.Key,
            //        TotalQuantity = g.Sum(x => x.Quantity),
            //        TotalRevenue = g.Sum(x => x.Quantity * x.Price)
            //    })
            //    .OrderByDescending(x => x.TotalRevenue)
            //    .ToList();

            //dgvSalesLog.DataSource = query;
        }

        private void SalesLogForm_Load(object sender, EventArgs e)
        {
            string logFile = Path.Combine(Application.StartupPath, "sales_log.csv");

            if (!File.Exists(logFile))
            {
                MessageBox.Show("A naplófájl nem található.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var lines = File.ReadAllLines(logFile).Skip(1); // fejléc kihagyása

                var allRecords = lines
                    .Select(line => line.Split(';'))
                    .Where(parts => parts.Length >= 4)
                    .Select(parts => new
                    {
                        SKU = parts[0],
                        Quantity = int.Parse(parts[1]),
                        SaleDate = DateTime.Parse(parts[2]),
                        Price = decimal.Parse(parts[3])
                    })
                    .GroupBy(x => x.SKU)
                    .Select(g => new
                    {
                        SKU = g.Key,
                        TotalQuantity = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToList();

                dgvSalesLog.DataSource = allRecords;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba az eladási napló betöltésekor:\n" + ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
