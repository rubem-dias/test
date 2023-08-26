using Application.Services.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

                var fileName = Orders.FileName;

                var result = await _orderSerivice.ImportOrders(stream, fileName);

                _logger.LogInformation("Import done!");
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
                var OrderList = _mapper.Map<List<Order>>(OrderFileInput);
                var result = await _orderSerivice.CalculatePriceAndDeliveryDate(OrderList);

                _logger.LogInformation("Order done!");
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

                _logger.LogInformation("Approved orders!");
                return Ok();

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("/GetOrders")]
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