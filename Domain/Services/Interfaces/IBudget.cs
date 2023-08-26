using Domain.Models;

namespace Domain.Services.Interfaces
{
    public interface IBudget
    {
        public double DeliveryDistanceMultiplier(string ZipCode);
        public double DiscountDates(DateTime OrderDate);
        public List<Order> NewOrderValues(List<Order> OrderList, List<CepResponse> Cep, List<Product> Products);
    }
}