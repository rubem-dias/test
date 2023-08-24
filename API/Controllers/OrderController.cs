using Application.Services.Interfaces;
using AutoMapper;
using Domain;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Persistence;

namespace API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly ILogger<OrderController> _logger;
        private IOrderService _orderSerivice;
        private IMapper _mapper;
        public OrderController(ILogger<OrderController> logger, IOrderService orderService, IMapper mapper)
        {
            _logger = logger;
            _orderSerivice = orderService;
            _mapper = mapper;
        }

        [HttpPost("/ImportOrder")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Order>>> ImportOrder(IFormFile Orders) 
        {
            _logger.LogInformation("Trying to post orders...");

            try 
            {
                if (Orders == null)
                    return BadRequest("File is empty");
                
                using MemoryStream stream = new MemoryStream();
                await Orders.CopyToAsync(stream);
                stream.Position = 0;

                await _orderSerivice.ImportOrders(stream);

                return Ok();

            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("/OrderHandler")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Order>>> OrderHandler(List<OrderInput> OrderInput) 
        {
            _logger.LogInformation("Trying to calculate orders...");

            try 
            {
                List<Order> OrderList = new List<Order>();

                using StreamReader reader = new("/home/rubem/Downloads/ddd-webapi-master/OnionApp/Persistence/Mock/mock.json");
                var json = reader.ReadToEnd();
                List<OrderInput> Orders = JsonConvert.DeserializeObject<List<OrderInput>>(json);

                foreach(var o in Orders)
                {
                    OrderList.Add(new Order() 
                    {
                        CorporateName = o.CorporateName,
                        Document = o.Document,
                        ZipCode = o.ZipCode,
                        Product = o.Product,
                        OrderNumber = o.OrderNumber,
                        DateOrdered = o.DateOrdered
                    });
                }

                await _orderSerivice.CalculateAndDeliveryDate(OrderList);

                return Ok();

            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}