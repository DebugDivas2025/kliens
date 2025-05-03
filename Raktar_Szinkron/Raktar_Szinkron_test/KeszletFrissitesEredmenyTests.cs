using Xunit;
using Raktar_Szinkron.Models;

namespace Raktar_Szinkron_test.Models
{
    public class KeszletFrissitesEredmenyTests
    {
        [Fact]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            var eredmeny = new KeszletFrissitesEredmeny
            {
                Sikeres = true,
                EredetiKeszlet = 5,
                UjKeszlet = 10,
                Ar = 34990m
            };

            Assert.True(eredmeny.Sikeres);
            Assert.Equal(5, eredmeny.EredetiKeszlet);
            Assert.Equal(10, eredmeny.UjKeszlet);
            Assert.Equal(34990m, eredmeny.Ar);
        }

        [Fact]
        public void Default_Sikeres_IsFalse()
        {
            var eredmeny = new KeszletFrissitesEredmeny();
            Assert.False(eredmeny.Sikeres);
        }

        [Fact]
        public void Default_Ar_IsZero()
        {
            var eredmeny = new KeszletFrissitesEredmeny();
            Assert.Equal(0m, eredmeny.Ar);
        }

        [Fact]
        public void Default_EredetiKeszlet_And_UjKeszlet_AreZero()
        {
            var eredmeny = new KeszletFrissitesEredmeny();
            Assert.Equal(0, eredmeny.EredetiKeszlet);
            Assert.Equal(0, eredmeny.UjKeszlet);
        }

        [Fact]
        public void DifferenceInKeszlet_IsCalculatedCorrectly()
        {
            var eredmeny = new KeszletFrissitesEredmeny
            {
                EredetiKeszlet = 3,
                UjKeszlet = 8
            };

            var kulonbseg = eredmeny.UjKeszlet - eredmeny.EredetiKeszlet;
            Assert.Equal(5, kulonbseg);
        }
    }
}
