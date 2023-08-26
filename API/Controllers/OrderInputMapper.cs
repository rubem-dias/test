using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class OrderInputMapper : Profile
    {
        public OrderInputMapper() => CreateMap<OrderFileInput, Order>()
            .ForMember(d => d.Document, o => o.MapFrom(s => s.Document))
            .ForMember(d => d.CorporateName, o => o.MapFrom(s => s.CorporateName))
            .ForMember(d => d.ZipCode, o => o.MapFrom(s => s.ZipCode))
            .ForMember(d => d.Product, o => o.MapFrom(s => s.Product))
            .ForMember(d => d.OrderNumber, o => o.MapFrom(s => s.OrderNumber))
            .ForMember(d => d.DateOrdered, o => o.MapFrom(s => s.DateOrdered))
            .ForMember(d => d.PriceWithDelivery, o => o.Ignore())
            .ForMember(d => d.PriceWithoutDelivery, o => o.Ignore())
            .ForMember(d => d.EstimatedDelivery, o => o.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore());
    }
}