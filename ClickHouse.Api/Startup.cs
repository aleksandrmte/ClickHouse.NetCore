using ClickHouse.Client.ADO;
using ClickHouse.NetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClickHouse.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            const string chConnectionString = "Host=192.168.1.104;User=default;Password=;Database=testdb;";

            services.AddTransient(sp => new ClickHouseConnection(chConnectionString));
            services.AddTransient<IClickHouseCommandFormatter, ClickHouseCommandFormatter>();
            services.AddTransient<IPropertyBinder, DefaultPropertyBinder>();
            services.AddTransient<IClickHouseDatabase, ClickHouseDatabase>();
            services.AddTransient<ClickHouseService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
