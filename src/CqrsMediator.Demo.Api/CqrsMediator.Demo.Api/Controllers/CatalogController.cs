using System.Collections.Generic;

using AutoMapper;

using CqrsMediator.Demo.Api.Dto;
using CqrsMediator.Demo.Bll.Services;

using Microsoft.AspNetCore.Mvc;

namespace CqrsMediator.Demo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private IMapper _mapper;

        public CatalogController(ICatalogService catalogService, IMapper mapper)
        {
            _catalogService = catalogService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<Dto.Product>> GetProducts([FromQuery] string name = null, [FromQuery] string description = null)
        {
            return _mapper.Map<List<Dto.Product>>(_catalogService.FindProducts(name, description));
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
