using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.Newtonsoft;
using Swashbuckle.AspNetCore.SwaggerGen;

using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;

using StockDataForNseApi.Model;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Data.SqlClient;

namespace StockDataForNseApi
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
            services.AddControllers();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //services.AddCors(c =>
            //{
            //    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            //});



            //below lineof code for enebeling cores as front en is angular different from dot net api
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            //services.AddCors(o => o.AddPolicy("OraclePolicy", builder =>
            //{
            //    builder.AllowAnyOrigin()
            //           .AllowAnyMethod()
            //           .AllowAnyHeader();
            //}));


            // var connection = @"Server=DESKTOP-2EA3GVK\DOTNETCOREXPRESS;Database=StockData;Trusted_Connection=True;ConnectRetryCount=0";
            var connectionFromAppseetings = Configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionFromAppseetings);
            //  services.AddDbContext<StockDataContext>(options => options.UseSqlServer(connection));
            services.AddDbContext<StockDataContext>(options => options.UseSqlServer(connectionFromAppseetings));
            

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            //app.UseCors(options => options.AllowAnyOrigin());
           
            //below lineof code for enebeling cores as front en is angular different from dot net api
            app.UseCors("MyPolicy");
            ////app.UseCors("OraclePolicy");
            //******************
            app.UseRouting();

            app.UseAuthorization();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           
          //  app.UseMvc();
        }
    }
}
