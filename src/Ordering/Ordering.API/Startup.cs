using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Application.Handlers;
using Ordering.Core.Repositories;
using Ordering.Core.Repositories.Base;
using Ordering.Infrastructure.Data;
using System.Reflection;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Producer;
using Microsoft.OpenApi.Models;
using Ordering.Infrastructure.Repositories;
using Ordering.Infrastructure.Repositories.Base;
using RabbitMQ.Client;

namespace Ordering.API
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

            // use real database
            services.AddDbContext<OrderContext>(c =>
                c.UseSqlServer(Configuration.GetConnectionString("OrderConnection")), ServiceLifetime.Singleton);
            // we made singleton this in order to resolve in mediatR when consuming Rabbit

            services.AddAutoMapper(typeof(Startup));

            services.AddSingleton<IRabbitMQConnection>(sp => {

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBus:Hostname"],
                    UserName = Configuration["EventBus:UserName"],
                    Password = Configuration["EventBus:Password"]
                };

                return new RabbitMQConnection(factory);

            });

            services.AddSingleton<EventBusRabbitMQProducer>();

            services.AddMediatR(typeof(CheckoutOrderHandler).GetTypeInfo().Assembly);



            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo {Title = "Order API", Version = "v1"});
            });
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
            });
        }
    }
}
