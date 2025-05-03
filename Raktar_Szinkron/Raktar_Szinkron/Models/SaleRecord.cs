using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Raktar_Szinkron.Models
{
    public class SaleRecord
    {
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Price { get; set; }
        public int OriginalQuantity { get; set; }
        public int UpdatedQuantity { get; set; }
        public bool Szinkronizalva { get; set; }

        //public Image ProductImage { get; set; }
        //public bool Synced { get; set; }
    }
}
