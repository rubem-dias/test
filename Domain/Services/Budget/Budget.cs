using Domain.Models;
using Domain.Services.Interfaces;

namespace Domain.Services.Budget
{
    public class Budget : IBudget
    {
        public double DeliveryDistanceMultiplier(Order Order, CepResponse Cep)
        {
            int ZipCodeToCompare = Int32.Parse(Order.ZipCode);
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

        public double DiscountDates(Order Order)
        {

            double Percentage = 0;

            List<DateTime> Christmas = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(Order.DateOrdered.Year, 12, 1).AddDays(1))
                .TakeWhile(e => new DateTime(Order.DateOrdered.Year, 12, 31) <= e)
                .ToList();

            List<DateTime> BlackFriday = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(Order.DateOrdered.Year, 11, 25).AddDays(1))
                .TakeWhile(e => new DateTime(Order.DateOrdered.Year, 11, 30) <= e)
                .ToList();

            List<DateTime> FathersDay = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(Order.DateOrdered.Year, 8, 1).AddDays(1))
                .TakeWhile(e => new DateTime(Order.DateOrdered.Year, 8, 15) <= e)
                .ToList();

            List<DateTime> MothersDay = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(Order.DateOrdered.Year, 5, 1).AddDays(1))
                .TakeWhile(e => new DateTime(Order.DateOrdered.Year, 5, 15) <= e)
                .ToList();
            
            if (Order.DateOrdered >= Christmas.FirstOrDefault() && Order.DateOrdered <= Christmas.LastOrDefault())
            {
                Percentage = 0.10;
            }
            else if (Order.DateOrdered >= BlackFriday.FirstOrDefault() && Order.DateOrdered <= BlackFriday.LastOrDefault())
            {
                Percentage = 0.30;
            } else if (Order.DateOrdered >= FathersDay.FirstOrDefault() && Order.DateOrdered <= FathersDay.LastOrDefault())
            {
                Percentage = 0.05;
            } else if (Order.DateOrdered >= MothersDay.FirstOrDefault() && Order.DateOrdered <= MothersDay.LastOrDefault())
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

                var MultiplierByLocation = DeliveryDistanceMultiplier(order, cep);
                var DiscountByDate = DiscountDates(order);

                if (DiscountByDate != 0)
                {
                    var discount = product.Price * DiscountByDate;
                    var priceWithDiscount = product.Price - discount;

                    if (MultiplierByLocation != 0)
                    {
                        var priceWithLocation = priceWithDiscount * MultiplierByLocation;
                        var finalPrice = product.Price = priceWithLocation;

                        order.PriceWithoutDelivery = product.Price;
                        order.PriceWithDelivery = finalPrice;
                    }
                } else if (DiscountByDate == 0 && MultiplierByLocation != 0)
                {
                    order.PriceWithDelivery = product.Price * MultiplierByLocation;
                    order.PriceWithoutDelivery = product.Price;
                } else 
                {
                    order.PriceWithDelivery = product.Price;
                    order.PriceWithoutDelivery = product.Price;
                }
            }

            return OrderList;
        }
    }
}