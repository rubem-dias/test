using Domain.Models;

namespace Domain.Services.Interfaces
{
    public interface IBudget
    {
        public double DeliveryDistanceMultiplier(Order Order, CepResponse Cep);
        public double DiscountDates(Order Order);
        public List<Order> NewOrderValues(List<Order> OrderList, List<CepResponse> Cep, List<Product> Products);
    }
}