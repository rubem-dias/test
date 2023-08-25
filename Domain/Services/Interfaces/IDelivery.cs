using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Services.Interfaces
{
    public interface IDelivery
    {
        public int DeliveryDayByLocation(string Cep);
        public int DeliveryByProduct(string ProductName);
        public int DeliveryByDate(DateTime OrderedDate);
        public List<Order> OrdersWithDeliveryDate(List<Order> Orders, List<CepResponse> Cep);

    }
}