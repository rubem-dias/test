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

            if (OrderedDate >= new DateTime(OrderedDate.Year, 12, 1) && OrderedDate <= new DateTime(OrderedDate.Year, 12, 31))
            {
                UtilDaysCount = 10;
            }
            else if (OrderedDate >= new DateTime(OrderedDate.Year, 11, 25) && OrderedDate <= new DateTime(OrderedDate.Year, 11, 30))
            {
                UtilDaysCount = 15;
            }
            else if (OrderedDate >= new DateTime(OrderedDate.Year, 8, 1) && OrderedDate <= new DateTime(OrderedDate.Year, 8, 15))
            {
                UtilDaysCount = 3;
            }
            else if (OrderedDate >= new DateTime(OrderedDate.Year, 5, 1) && OrderedDate <= new DateTime(OrderedDate.Year, 5, 15))
            {
                UtilDaysCount = 3;
            }
            else
            {
                return 0;
            }

            return UtilDaysCount;
        }

        public List<Order> OrdersWithDeliveryDate(List<Order> Orders, List<CepResponse> Cep)
        {
            foreach(var order in Orders)
            {
                string ZipCode = Cep
                    .Where(x => x.Cep.Replace("-", string.Empty) == order.ZipCode)
                    .Select(x => x.Cep)
                    .First();

                int DeliveryLocation = DeliveryDayByLocation(order.ZipCode);
                int DeliveryProduct = DeliveryByProduct(order.Product);
                int DeliveryDate = DeliveryByDate(order.DateOrdered);

                int TotalDays = DeliveryLocation + DeliveryProduct + DeliveryDate;

                order.ZipCode = ZipCode;
                order.EstimatedDelivery = order.DateOrdered.AddDays(TotalDays);
            }

            return Orders;
        }

    }
}