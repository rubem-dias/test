using Application.Services.Interfaces;
using Domain.Models;
using Persistence;
using System.Text.Json;
using AutoMapper;
using ClosedXML.Excel;
using Domain.Services.Interfaces;
using Persistence.Http.Interfaces;
using System.Net;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IBudget _budget;
        private readonly IDelivery _delivery;
        private readonly IViaCep _viaCep;

        public OrderService(DataContext context, IMapper mapper, IBudget budget, IDelivery delivery, IViaCep viaCep)
        {
            _context = context;
            _mapper = mapper;
            _delivery = delivery;
            _budget = budget;
            _viaCep = viaCep;
        }

        public async Task<List<OrderFileInput>> ImportOrders(MemoryStream fileStream)
        {

            List<OrderFileInput> OrderInputs = new List<OrderFileInput>();

            var xls = new XLWorkbook(fileStream);
            var sheet = xls.Worksheets.First();
            var totalRow = sheet.RowsUsed().Count();

            for (int l = 2; l <= totalRow; l++)
            {
                OrderInputs.Add(new OrderFileInput()
                {
                    Document = NormalizeField(sheet.Cell($"A{l}").Value.ToString()),
                    CorporateName = NormalizeField(sheet.Cell($"B{l}").Value.ToString()),
                    ZipCode = NormalizeField(sheet.Cell($"C{l}").Value.ToString()),
                    Product = NormalizeField(sheet.Cell($"D{l}").Value.ToString()),
                    OrderNumber = NormalizeField(sheet.Cell($"E{l}").Value.ToString()),
                    DateOrdered = (DateTime)sheet.Cell($"F{l}").Value
                });
            }

            string DataToJson = JsonSerializer.Serialize(OrderInputs);
            File.WriteAllText(@"/home/rubem/Downloads/ddd-webapi-master/OnionApp/Persistence/Mock/mock.json", DataToJson);

            return OrderInputs;
        }

        public async Task<List<Order>>  CalculateAndDeliveryDate(List<Order> Orders)
        {
           var CompleteAddress = await _viaCep.GetAddress(Orders.Select(x => x.ZipCode).ToList());
           var Products = _context.Product.ToList();
        

           var OrderChangedPrice = _budget.NewOrderValues(Orders, CompleteAddress, Products);
           var OrdersWithDeliveryAndPrice = _delivery.OrdersWithDeliveryDate(OrderChangedPrice);

            string DataToJson = JsonSerializer.Serialize(OrdersWithDeliveryAndPrice);
            File.WriteAllText(@"/home/rubem/Downloads/ddd-webapi-master/OnionApp/Persistence/Mock/mock-newOrders.json", DataToJson);

           return OrdersWithDeliveryAndPrice;
        }

        public async Task<HttpStatusCode> PostOrder(List<Order> Orders)
        {
            try {
                await _context.Order.AddRangeAsync(Orders);
                await _context.SaveChangesAsync();

                return HttpStatusCode.OK;
            } catch (Exception)
            {
                return HttpStatusCode.NotModified;
            }
        }
        
        public async Task<List<Order>> GetOrders()
        {
            try 
            {
                var result =  _context.Order.ToList();
                return result;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        
        protected string NormalizeField(string Field)
        {
            if (!String.IsNullOrEmpty(Field))
                Field = Field.Replace(".", string.Empty)
                        .Replace("-", string.Empty)
                        .TrimStart()
                        .TrimEnd();

            return Field;
        }
    }
}