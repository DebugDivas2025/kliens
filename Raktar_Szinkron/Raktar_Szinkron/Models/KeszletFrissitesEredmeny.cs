using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raktar_Szinkron.Models
{
    public class KeszletFrissitesEredmeny
    {
        public bool Sikeres { get; set; }
        public int EredetiKeszlet { get; set; }
        public int UjKeszlet { get; set; }
        public decimal Ar { get; set; }
    }
}
