using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using CqrsMediator.Demo.Api.Dto;
using CqrsMediator.Demo.Bll.Catalog.Queries;
using CqrsMediator.Demo.Bll.Services;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace CqrsMediator.Demo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private IMapper _mapper;
        private readonly IMediator _mediator;

        public CatalogController(ICatalogService catalogService, IMapper mapper, IMediator mediator)
        {
            _catalogService = catalogService;
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Dto.Product>>> GetProducts([FromQuery] FindProduct.Query query)
        {
            return _mapper.Map<List<Dto.Product>>(await _mediator.Send(query));
        }

        [HttpGet("{productId:int}")]
        public ActionResult<Dto.Product> GetProduct(int productId)
        {
            return _mapper.Map<Dto.Product>(_catalogService.GetProduct(productId));
        }

        [HttpPost]
        public ActionResult CreateProduct([FromBody] CreateProductRequest request)
        {
            var p = _catalogService.CreateProduct(request.Name, request.Description, request.UnitPrice);
            return CreatedAtAction(nameof(GetProducts), new { productId = p.ProductId }, _mapper.Map<Dto.Product>(p));
        }
    }
}
