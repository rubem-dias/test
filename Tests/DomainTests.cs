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
    }
}