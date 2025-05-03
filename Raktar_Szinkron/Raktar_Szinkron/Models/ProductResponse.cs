using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raktar_Szinkron.Models
{
    public class ProductResponse
    {
        public ProductContent Content { get; set; }
    }
    public class ProductContent
    {
        public List<Product> Products { get; set; }
    }
}
