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
        public async Task<IActionResult> ImportOrder(IFormFile Orders) 
        {
            _logger.LogInformation("Trying to post orders...");

            try 
            {
                if (Orders == null)
                    return BadRequest("File is empty");
                
                using MemoryStream stream = new MemoryStream();
                await Orders.CopyToAsync(stream);
                stream.Position = 0;

                var result = await _orderSerivice.ImportOrders(stream);

                return Ok(result);

            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("/OrderHandler")]
        [AllowAnonymous]
        public async Task<IActionResult> OrderHandler(List<OrderFileInput> OrderFileInput) 
        {
            _logger.LogInformation("Trying to calculate orders...");

            try 
            {
                List<Order> OrderList = new List<Order>();

                foreach(var o in OrderFileInput)
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

                var result = await _orderSerivice.CalculatePriceAndDeliveryDate(OrderList);

                return Ok(result);

            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("/ApproveOrders")]
        [AllowAnonymous]
        public async Task<IActionResult> ApproveOrders(List<Order> Orders)

        {
            try 
            {
                var result = await _orderSerivice.PostOrder(Orders);
                return Ok();

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<Order>> GetOrders()
        {
            try
            {
                List<Order> result = await _orderSerivice.GetOrders();
                return result;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}