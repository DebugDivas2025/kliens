using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raktar_Szinkron.Models
{
    public class Product
    {
        public string Sku { get; set; }
        public int? QuantityOnHand { get; set; }
        public string ProductName { get; set; }
        public decimal SitePrice { get; set; }
        public string Bvin { get; set; }
        //public string ImageFileSmall { get; set; }

    }
}
