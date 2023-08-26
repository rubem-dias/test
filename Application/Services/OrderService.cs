using Application.Services.Interfaces;
using Domain.Models;
using Persistence;
using System.Text.Json;
using AutoMapper;
using ClosedXML.Excel;
using Domain.Services.Interfaces;
using Persistence.Http.Interfaces;
using System.Net;
using DocumentFormat.OpenXml.Office2021.PowerPoint.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;
using MMLib.Extensions;

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
                    Document = NormalizeField(sheet.Cell($"A{l}").Value.ToString(), "Documento"),
                    CorporateName = NormalizeField(sheet.Cell($"B{l}").Value.ToString(),"Razão social"),
                    ZipCode = NormalizeField(sheet.Cell($"C{l}").Value.ToString(), "Cep"),
                    Product = NormalizeField(sheet.Cell($"D{l}").Value.ToString(), "Produto"),
                    OrderNumber = NormalizeField(sheet.Cell($"E{l}").Value.ToString(), "Número do pedido"),
                    DateOrdered = (DateTime)sheet.Cell($"F{l}").Value
                });
            }

            return OrderInputs;
        }

        public async Task<List<Order>>  CalculatePriceAndDeliveryDate(List<Order> Orders)
        {
           var CompleteAddress = await _viaCep.GetAddress(Orders.Select(x => x.ZipCode).ToList());
           var Products = _context.Product.ToList();
        

           var OrderChangedPrice = _budget.NewOrderValues(Orders, CompleteAddress, Products);
           var OrdersWithDeliveryAndPrice = _delivery.OrdersWithDeliveryDate(OrderChangedPrice, CompleteAddress);

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
                var result =  _context.Order.OrderBy(x => x.CorporateName).ToList();
                return result;

            } catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        
        protected string NormalizeField(string Field, string FieldName)
        {
            Regex Reg = new Regex("[*'\",_&#^@]");

            if (!String.IsNullOrEmpty(Field))
            {
                Field = Field.Replace(".", string.Empty)
                        .Replace("-", string.Empty)
                        .TrimStart()
                        .TrimEnd();
                
                Field = Reg.Replace(Field, string.Empty);
            } else 
            { 
              throw new Exception($"Field cannot be empty. Field empty: {FieldName}");
            }

            return Field;
        }

    }
}