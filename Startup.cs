using BuildRestApiNetCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BuildRestApiNetCore
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<ProductContext>(opt => opt.UseInMemoryDatabase("Products"));
      services.AddControllers().AddNewtonsoftJson();
      services.AddApiVersioning(opt => opt.ReportApiVersions = true);

      services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "Products",
        Description = "The ultimate e-commerce store for all your needs",
        Version = "v1"
      }));
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseSwagger();
      app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Products v1"));

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
