using Domain.Models;
using Domain.Services.Interfaces;

namespace Domain.Services.Delivery
{
    public class Delivery : IDelivery
    {
        public int DeliveryDayByLocation(string ZipCode)
        {
            int ZipCodeToCompare = Int32.Parse(ZipCode);
            int UtilDaysCount;

            if (ZipCodeToCompare >= 01000000 && ZipCodeToCompare <= 39999999)
            {
                UtilDaysCount = 2;
            } else if (ZipCodeToCompare >= 40000000 && ZipCodeToCompare <= 77999999)
            {
                UtilDaysCount = 4;
            } else {
                UtilDaysCount = 3;
            }

            return UtilDaysCount;
        }

        public int DeliveryByProduct(string ProductName)
        {
            int UtilDaysCount;

            if (ProductName == "Televisão")
                UtilDaysCount = 3;
            else if (ProductName == "Notebook")
            {
                UtilDaysCount = 2;
            } else 
            {
                UtilDaysCount = 1;
            }

            return UtilDaysCount;
        }

        public int DeliveryByDate(DateTime OrderedDate)
        {

            int UtilDaysCount;

            List<DateTime> Christmas = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(OrderedDate.Year, 12, 1).AddDays(1))
                .TakeWhile(e => new DateTime(OrderedDate.Year, 12, 31) <= e)
                .ToList();

            List<DateTime> BlackFriday = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(OrderedDate.Year, 11, 25).AddDays(1))
                .TakeWhile(e => new DateTime(OrderedDate.Year, 11, 30) <= e)
                .ToList();

            List<DateTime> FathersDay = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(OrderedDate.Year, 8, 1).AddDays(1))
                .TakeWhile(e => new DateTime(OrderedDate.Year, 8, 15) <= e)
                .ToList();

            List<DateTime> MothersDay = Enumerable.Range(0, Int32.MaxValue)
                .Select(e => new DateTime(OrderedDate.Year, 5, 1).AddDays(1))
                .TakeWhile(e => new DateTime(OrderedDate.Year, 5, 15) <= e)
                .ToList();
            
            if (OrderedDate >= Christmas.FirstOrDefault() && OrderedDate <= Christmas.LastOrDefault())
            {
                UtilDaysCount = 10;
            }
            else if (OrderedDate >= BlackFriday.FirstOrDefault() && OrderedDate <= BlackFriday.LastOrDefault())
            {
                UtilDaysCount = 15;
            } else if (OrderedDate >= FathersDay.FirstOrDefault() && OrderedDate <= FathersDay.LastOrDefault())
            {
                UtilDaysCount = 3;
            } else if (OrderedDate >= MothersDay.FirstOrDefault() && OrderedDate <= MothersDay.LastOrDefault())
            {
                UtilDaysCount = 3;
            } else 
            {
                return 0;
            }

            return UtilDaysCount;
        }

        public List<Order> OrdersWithDeliveryDate(List<Order> Orders)
        {
            foreach(var order in Orders)
            {
                int DeliveryLocation = DeliveryDayByLocation(order.ZipCode);
                int DeliveryProduct = DeliveryByProduct(order.Product);
                int DeliveryDate = DeliveryByDate(order.DateOrdered);

                int TotalDays = DeliveryLocation + DeliveryProduct + DeliveryDate;

                order.EstimatedDelivery = order.DateOrdered.AddDays(TotalDays);
            }

            return Orders;
        }

    }
}