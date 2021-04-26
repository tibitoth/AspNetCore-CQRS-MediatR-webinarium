using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<List<Dto.Product>>> GetProducts([FromQuery] string name = null, [FromQuery] string description = null)
        {
            return _mapper.Map<List<Dto.Product>>(await _catalogService.FindProductsAsync(name, description));
        }

        [HttpGet("{productId:int}")]
        public async Task<ActionResult<Dto.Product>> GetProduct(int productId)
        {
            return _mapper.Map<Dto.Product>(await _catalogService.GetProductAsync(productId));
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var p = await _catalogService.CreateProductAsync(request.Name, request.Description, request.UnitPrice);
            return CreatedAtAction(nameof(GetProducts), new { productId = p.ProductId }, _mapper.Map<Dto.Product>(p));
        }
    }
}
