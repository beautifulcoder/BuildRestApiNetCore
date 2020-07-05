using Microsoft.AspNetCore.Mvc;

namespace BuildRestApiNetCore.Models
{
  public class ProductRequest
  {
    [FromQuery(Name = "limit")]
    public int Limit { get; set; } = 15;

    [FromQuery(Name = "offset")]
    public int Offset { get; set; }
  }
}
