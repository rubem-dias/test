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
using CsvHelper;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2016.Excel;

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

        public async Task<List<OrderFileInput>> ImportOrders(MemoryStream fileStream, string fileName)
        {
            if (fileName.Contains("xlsx"))
            {
                return await ImportXlsx(fileStream);
            } else
            {
                return await ImportCsv(fileStream);
            }
        }

        protected async Task<List<OrderFileInput>> ImportXlsx(MemoryStream fileStream)
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
                    CorporateName = NormalizeField(sheet.Cell($"B{l}").Value.ToString(), "Razão social"),
                    ZipCode = NormalizeField(sheet.Cell($"C{l}").Value.ToString(), "Cep"),
                    Product = NormalizeField(sheet.Cell($"D{l}").Value.ToString(), "Produto"),
                    OrderNumber = NormalizeField(sheet.Cell($"E{l}").Value.ToString(), "Número do pedido"),
                    DateOrdered = (DateTime)sheet.Cell($"F{l}").Value
                });
            }

            return OrderInputs;
        }

        protected async Task<List<OrderFileInput>> ImportCsv(MemoryStream fileStream)
        {
            List<OrderFileInput> OrderInputs = new List<OrderFileInput>();

            using (StreamReader sr = new StreamReader(fileStream))
            {
                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (currentLine.Contains("Documento,Razão Social,CEP,Produto,Número do pedido,Data"))
                    {
                        continue;
                    }

                    var fields = currentLine.Split(",");
                    var BadDateTimeFormat = fields[5];

                    var cultureInfo = new CultureInfo("pt-BR");
                    var DateParsed = DateTime.Parse(BadDateTimeFormat, cultureInfo);

                    OrderInputs.Add(new OrderFileInput()
                    {
                        Document = NormalizeField(fields[0], "Documento"),
                        CorporateName = NormalizeField(fields[1], "Razão social"),
                        ZipCode = NormalizeField(fields[2], "Cep"),
                        Product = NormalizeField(fields[3], "Produto"),
                        OrderNumber = NormalizeField(fields[4], "Número do pedido"),
                        DateOrdered = DateParsed
                    });

                }
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
                Field = Reg.Replace(Field, string.Empty);
            } else 
            { 
              throw new Exception($"Field cannot be empty. Field empty: {FieldName}");
            }

            return Field;
        }
    }
}