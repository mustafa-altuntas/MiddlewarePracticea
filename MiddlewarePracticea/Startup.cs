using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MiddlewarePracticea.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddlewarePracticea
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiddlewarePracticea", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiddlewarePracticea v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();



            //Custome middleware
            app.UseHello();


            //app.Use() -> kendi iþlemini yapýyor next.Invoke() methodu ile bir sonraki middleware'a aktarým yapýyor benden sontraki çalýþsýn diyor
            app.Use(async (context, next) =>
            {
                Console.WriteLine("Middleware 1 baþladý.");
                await next.Invoke();
                Console.WriteLine("Middleware 1 sonlandýrýlýyor.");
            });

            app.Use(async (context, next) =>
            {
                Console.WriteLine("Middleware 2 baþladý.");
                await next.Invoke();
                Console.WriteLine("Middleware 2 sonlandýrýlýyor.");
            });


            app.Use(async (context, next) =>
            {
                Console.WriteLine("Middleware 3 baþladý.");
                await next.Invoke();
                Console.WriteLine("Middleware 3 sonlandýrýlýyor.");
            });


            app.Use(async (context, next) =>
            {
                Console.WriteLine("Use middleware tetiklendi.");
                await next.Invoke();
            });


            //app.Map() -> routa göre middlewareleri yönetmemizi saðlar örneðin /anasayfa routuna bir istek gelirse þunu middleware'i çalýþtir
            app.Map("/example", internalApp => internalApp.Run(async context =>
             {
                 Console.WriteLine("/example middleware tetiklendi.");
                 await context.Response.WriteAsync("example middleware tetiklendi.");
             }));


            //app.MapWhen() request içerisindeki herhangi bir parametreye göre middleware çalýþtýrma
            app.MapWhen(x => x.Request.Method == "GET", internalApp => internalApp.Run(async context =>
            {
                Console.WriteLine("MapWhen Middleware tetiklendi.");
                await context.Response.WriteAsync("MapWhen Middleware tetiklendi.");
            }));








            // Kýsa devre yaptýrýr kendinden sonra gelen middleware'larýn çalýþmamasýný saðlar. 
            //app.Run(async context => Console.WriteLine("Middleware 1."));
            //app.Run(async context => Console.WriteLine("Middleware 1."));


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
