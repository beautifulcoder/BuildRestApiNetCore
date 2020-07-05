using System;
using System.Linq;
using BuildRestApiNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildRestApiNetCore.Controllers
{
  [ApiController]
  [ApiVersion("1.0")]
  [Route("v{version:apiVersion}/[controller]")]
  [Produces("application/json")]
  public class ProductsController : ControllerBase
  {
    private readonly ProductContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ProductContext context, ILogger<ProductsController> logger)
    {
      _context = context;
      _logger = logger;

      if (_context.Products.Any()) return;

      ProductSeed.InitData(context);
    }

    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IQueryable<Product>> GetProducts([FromQuery] string department, [FromQuery] ProductRequest request)
    {
      if (request.Limit >= 100) _logger.LogInformation("Requesting more than 100 products.");

      var result = _context.Products as IQueryable<Product>;

      if (!string.IsNullOrEmpty(department))
      {
        result = result.Where(p => p.Department.StartsWith(department, StringComparison.InvariantCultureIgnoreCase));
      }

      Response.Headers["x-total-count"] = result.Count().ToString();

      return Ok(result
        .OrderBy(p => p.ProductNumber)
        .Skip(request.Offset)
        .Take(request.Limit));
    }

    [HttpGet]
    [Route("{productNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetProductByProductNumber([FromRoute] string productNumber)
    {
      var productDb = _context.Products
        .FirstOrDefault(p => p.ProductNumber.Equals(productNumber, StringComparison.InvariantCultureIgnoreCase));

      if (productDb == null) return NotFound();

      return Ok(productDb);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> PostProduct([FromBody] Product product)
    {
      try
      {
        _context.Products.Add(product);
        _context.SaveChanges();

        return new CreatedResult($"/products/{product.ProductNumber.ToLower()}", product);
      }
      catch (Exception e)
      {
        _logger.LogWarning(e, "Unable to POST product.");

        return ValidationProblem(e.Message);
      }
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> PutProduct([FromBody] Product product)
    {
      try
      {
        var productDb = _context.Products
          .FirstOrDefault(p => p.ProductNumber.Equals(product.ProductNumber, StringComparison.InvariantCultureIgnoreCase));

        if (productDb == null) return NotFound();

        productDb.Name = product.Name;
        productDb.Price = product.Price;
        productDb.Department = product.Department;
        _context.SaveChanges();

        return Ok(product);
      }
      catch (Exception e)
      {
        _logger.LogWarning(e, "Unable to PUT product.");

        return ValidationProblem(e.Message);
      }
    }

    [HttpPatch]
    [Route("{productNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> PatchProduct([FromRoute] string productNumber, [FromBody] JsonPatchDocument<Product> patch)
    {
      try
      {
        var productDb = _context.Products
          .FirstOrDefault(p => p.ProductNumber.Equals(productNumber, StringComparison.InvariantCultureIgnoreCase));

        if (productDb == null) return NotFound();

        patch.ApplyTo(productDb, ModelState);

        if (!ModelState.IsValid || !TryValidateModel(productDb)) return ValidationProblem(ModelState);

        _context.SaveChanges();

        return Ok(productDb);
      }
      catch (Exception e)
      {
        _logger.LogWarning(e, "Unable to PATCH product.");

        return ValidationProblem(e.Message);
      }
    }

    [HttpDelete]
    [Route("{productNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> DeleteProduct([FromRoute] string productNumber)
    {
      var productDb = _context.Products
        .FirstOrDefault(p => p.ProductNumber.Equals(productNumber, StringComparison.InvariantCultureIgnoreCase));

      if (productDb == null) return NotFound();

      _context.Products.Remove(productDb);
      _context.SaveChanges();

      return NoContent();
    }
  }
}
