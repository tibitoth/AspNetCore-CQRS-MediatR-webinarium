using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using CqrsMediator.Demo.Bll.Order.Commands;
using CqrsMediator.Demo.Bll.Services;
using CqrsMediator.Demo.Dal.Enum;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace CqrsMediator.Demo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private IMapper _mapper;
        private readonly IMediator _mediator;

        public OrderController(IOrderService orderService, IMapper mapper, IMediator mediator)
        {
            _orderService = orderService;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult<List<Dto.Order>> GetOrders([FromQuery] OrderStatus? status = null)
        {
            return _mapper.Map<List<Dto.Order>>(_orderService.FindOrders(status));
        }

        [HttpGet("{orderId:int}")]
        public ActionResult<Dto.Order> GetOrder(int orderId)
        {
            return _mapper.Map<Dto.Order>(_orderService.GetOrder(orderId));
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] CreateOrder.Command request)
        {
            var o = await _mediator.Send(request);
            return CreatedAtAction(nameof(GetOrder), new { orderId = o.OrderId }, _mapper.Map<Dto.Order>(o));
        }
    }
}
