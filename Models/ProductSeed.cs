using System;
using System.Linq;
using BuildRestApiNetCore.Extensions;

namespace BuildRestApiNetCore.Models
{
  public static class ProductSeed
  {
    public static void InitData(ProductContext context)
    {
      var rnd = new Random();

      var adjectives = new [] { "Small", "Ergonomic", "Rustic", "Smart", "Sleek" };
      var materials = new [] { "Steel", "Wooden", "Concrete", "Plastic", "Granite", "Rubber" };
      var names = new [] { "Chair", "Car", "Computer", "Pants", "Shoes" };
      var departments = new [] { "Books", "Movies", "Music", "Games", "Electronics" };

      context.Products.AddRange(900.Times(x =>
      {
        var adjective = adjectives[rnd.Next(0, 5)];
        var material = materials[rnd.Next(0, 5)];
        var name = names[rnd.Next(0, 5)];
        var department = departments[rnd.Next(0, 5)];
        var productId = $"{x, -3:000}";

        return new Product
        {
          ProductNumber = $"{department.First()}{name.First()}{productId}",
          Name = $"{adjective} {material} {name}",
          Price = (double) rnd.Next(1000, 9000) / 100,
          Department = department
        };
      }));

      context.SaveChanges();
    }
  }
}
