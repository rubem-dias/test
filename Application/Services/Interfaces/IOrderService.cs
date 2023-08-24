using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderInput>> ImportOrders(MemoryStream fileStream);
        Task<List<Order>> CalculateAndDeliveryDate(List<Order> OrderInput);
        
    }
}