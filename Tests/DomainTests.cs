using Domain.Models;
using Domain.Services.Budget;
using Domain.Services.Delivery;

namespace Tests
{
    public class DomainTests
    {

        [Fact]
        public void ShuldCorrectRegionFeesDelivery()
        {
            //MULTIPLIER 1.1 SUDESTE
            //CENTRO-OESTE/SUL 1.2
            //NORTE/NORDESTE 1.3

            var budget = new Budget();
            var Multiplier = budget.DeliveryDistanceMultiplier("59040045");

            Assert.Equal(1.3, Multiplier);

        }

        [Fact]
        public void ShouldCorrectDiscountPercentage()
        {
            var budget = new Budget();
            var Discount = budget.DiscountDates(new DateTime(2022, 08, 15));

            Assert.Equal(0.05, Discount);

        }

        [Fact]
        public void ShouldCorrectDeliveryDate()
        {
            var delivery = new Delivery();
            var deliveryDayAdd = delivery.DeliveryDayByLocation("30626660");

            Assert.Equal(2, deliveryDayAdd);

        }

        [Fact]
        public void ShouldHaveCorrectPricesOrder()
        {
            var budget = new Budget();

            List<Order> orderList = new List<Order>()
            {
                new Order()
                {
                    ZipCode = "30626660",
                    Product = "Celular",
                    DateOrdered = new DateTime(2022, 12, 05)
                }
            };
            List<Product> productList = new List<Product>()
            {
                new Product()
                {
                    Price = 1000,
                    Name = "Celular"
                }
            };
            List<CepResponse> cepList = new List<CepResponse>()
            {
                new CepResponse()
                {
                    Cep = "30626660"
                }
            };

            var OrderList = budget.NewOrderValues(orderList, cepList, productList);

            Assert.Equal(990, OrderList.FirstOrDefault().PriceWithDelivery);
            Assert.Equal(900, OrderList.FirstOrDefault().PriceWithoutDelivery);
        }

    }
}