using Application.Services.Interfaces;
using Domain.Models;
using Persistence;
using CsvHelper;
using System.Globalization;
using System.Text.Json;
using AutoMapper;
using CsvHelper.Configuration;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using MMLib.Extensions;
using Domain.Services.Interfaces;
using Persistence.Http;
using Persistence.Http.Interfaces;
using DocumentFormat.OpenXml.InkML;

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

        public async Task<List<OrderInput>> ImportOrders(MemoryStream fileStream)
        {

            List<OrderInput> OrderInputs = new List<OrderInput>();

            var xls = new XLWorkbook(fileStream);
            var sheet = xls.Worksheets.First();
            var totalRow = sheet.RowsUsed().Count();

            for (int l = 2; l <= totalRow; l++)
            {
                OrderInputs.Add(new OrderInput()
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
           return null;
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