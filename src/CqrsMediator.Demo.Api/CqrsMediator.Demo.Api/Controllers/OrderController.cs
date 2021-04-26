using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using CqrsMediator.Demo.Api.Dto;
using CqrsMediator.Demo.Bll.Services;
using CqrsMediator.Demo.Dal.Enum;

using Microsoft.AspNetCore.Mvc;

namespace CqrsMediator.Demo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Dto.Order>>> GetOrders([FromQuery] OrderStatus? status = null)
        {
            return _mapper.Map<List<Dto.Order>>(await _orderService.FindOrdersAsync(status));
        }

        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<Dto.Order>> GetOrder(int orderId)
        {
            return _mapper.Map<Dto.Order>(await _orderService.GetOrderAsync(orderId));
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] CreateOrderRequest request)
        {
            var o = await _orderService.CreateOrderAsync(
                request.CustomerName,
                request.CustomerAddress,
                request.OrderItems.ToDictionary(oi => oi.ProductId, oi => oi.Amount));
            return CreatedAtAction(nameof(GetOrder), new { orderId = o.OrderId }, _mapper.Map<Dto.Order>(o));
        }
    }
}
