using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Document { get; set; }
        public string CorporateName { get; set; }
        public string ZipCode { get; set; } 
        public string Product { get; set; }
        public string OrderNumber { get; set; }
        public DateTime DateOrdered { get; set; }
        public double PriceWithDelivery { get; set; }
        public double PriceWithoutDelivery { get; set; }
        public DateTime EstimatedDelivery { get; set; }
    }
}