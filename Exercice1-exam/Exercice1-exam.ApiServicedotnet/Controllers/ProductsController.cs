using Exercice1_exam.ApiServicedotnet.DTO;
using Exercice1_exam.ApiServicedotnet.Services;
using Microsoft.AspNetCore.Mvc;

namespace Exercice1_exam.ApiServicedotnet.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IStockService _stockService;

    public ProductsController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProductDto>> GetProducts()
    {
        var products = _stockService.GetAllProducts()
            .Select(p => new ProductDto(
                id: p.Id,
                name: p.Name,
                price: p.UnitPrice,
                stock: p.Stock
            ));

        return Ok(products);
    }
}