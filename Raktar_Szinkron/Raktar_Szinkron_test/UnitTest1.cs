using Xunit;
using Raktar_Szinkron.Models;

namespace Raktar_Szinkron.Tests.Models
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_SetsProductNameAndSku()
        {
            // Arrange & Act
            var product = new Product
            {
                ProductName = "Kávéfõzõ",
                Sku = "KF123"
            };

            // Assert
            Assert.Equal("Kávéfõzõ", product.ProductName);
            Assert.Equal("KF123", product.Sku);
        }

        [Fact]
        public void Default_QuantityOnHand_IsZero()
        {
            var product = new Product();
            Assert.Equal(0, product.QuantityOnHand);
        }

        [Fact]
        public void SitePrice_SetAndGetValue_Correct()
        {
            var product = new Product
            {
                SitePrice = 24999m
            };

            Assert.Equal(24999m, product.SitePrice);
        }

        [Fact]
        public void Default_QuantityOnHand_IsNull()
        {
            var product = new Product();
            Assert.Null(product.QuantityOnHand);
        }
    }
}