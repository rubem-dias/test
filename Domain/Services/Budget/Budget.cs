using Domain.Models;
using Domain.Services.Interfaces;

namespace Domain.Services.Budget
{
    public class Budget : IBudget
    {
        public double DeliveryDistanceMultiplier(string ZipCode)
        {
            int ZipCodeToCompare = Int32.Parse(ZipCode);
            double multiplier = 0;

            if (ZipCodeToCompare >= 01000000 && ZipCodeToCompare <= 39999999)
            {
                multiplier = 1.1;
            } else if (ZipCodeToCompare >= 40000000 && ZipCodeToCompare <= 77999999)
            {
                multiplier = 1.3;
            } else {
                multiplier = 1.2;
            }

            return multiplier;
        }

        public double DiscountDates(DateTime OrderDate)
        {

            double Percentage = 0;
            
            if (OrderDate >= new DateTime(OrderDate.Year, 12, 1) && OrderDate <= new DateTime(OrderDate.Year, 12, 31))
            {
                Percentage = 0.10;
            }
            else if (OrderDate >= new DateTime(OrderDate.Year, 11, 25) && OrderDate <= new DateTime(OrderDate.Year, 11, 30))
            {
                Percentage = 0.30;
            } else if (OrderDate >= new DateTime(OrderDate.Year, 8, 1) && OrderDate <= new DateTime(OrderDate.Year, 8, 15))
            {
                Percentage = 0.05;
            } else if (OrderDate >= new DateTime(OrderDate.Year, 5, 1) && OrderDate <= new DateTime(OrderDate.Year, 5, 15))
            {
                Percentage = 0.05;
            } else 
            {
                return Percentage;
            }

            return Percentage;
        }

        public List<Order> NewOrderValues(List<Order> OrderList, List<CepResponse> Cep, List<Product> Products)
        {
            foreach(var order in OrderList)
            {
                var cep = Cep.Where(x => x.Cep.Replace("-", string.Empty) == order.ZipCode).FirstOrDefault();
                var product = Products.Where(x => x.Name == order.Product).FirstOrDefault();

                var MultiplierByLocation = DeliveryDistanceMultiplier(order.ZipCode);
                var DiscountByDate = DiscountDates(order.DateOrdered);

                if (DiscountByDate != 0)
                {
                    var discount = product.Price * DiscountByDate;
                    var priceWithDiscount = product.Price - discount;

                    order.PriceWithDelivery = priceWithDiscount * MultiplierByLocation;
                    order.PriceWithoutDelivery = priceWithDiscount;

                } else
                {
                    order.PriceWithDelivery = product.Price * MultiplierByLocation;
                    order.PriceWithoutDelivery = product.Price;
                }

                order.PriceWithDelivery = ClearTrailingZeros(order.PriceWithDelivery);
                order.PriceWithoutDelivery = ClearTrailingZeros(order.PriceWithoutDelivery);
            }

            return OrderList;
        }

        protected double ClearTrailingZeros(double Price)
        {
            string strNumber = Price.ToString("N2");
            double newPriceFormatted = Convert.ToDouble(strNumber);

            return newPriceFormatted;
        }

    }
}