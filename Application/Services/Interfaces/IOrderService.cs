using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderFileInput>> ImportOrders(MemoryStream fileStream);
        Task<List<Order>> CalculatePriceAndDeliveryDate(List<Order> OrderFileInput);
        Task<HttpStatusCode> PostOrder(List<Order> Orders);
        Task<List<Order>> GetOrders();
        
    }
}