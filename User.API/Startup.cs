using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.API.Database;
using Microsoft.EntityFrameworkCore;
using Microservice.BuildingBlocks.EventBus.Abstractions;
using Microservice.BuildingBlocks.EventBus;
using Microservice.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System;
using User.API.Repositories;

namespace User.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //var contextOptions = new DbContextOptionsBuilder<EfContext>().UseSqlServer(Configuration.GetConnectionString("User.API.DbConnStr")).Options;

            //services.AddSingleton(contextOptions).AddScoped<EfContext>();
            //services.AddDbContext<EfContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("User.API.DbConnStr")));
            services.AddDbContext<EfContext>(opt => opt.UseSqlServer("Data Source=.;Initial Catalog=Microservice;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456"));

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersisterConnection>>();
                var factory = new ConnectionFactory() { HostName = "localhost" };

                return new DefaultRabbitMQPersisterConnection(factory, logger);
            });

            RegisterEventBus(services);

            services.AddTransient<IUserRepository, UserRepository>();

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }


        /// <summary>
        /// 注册EventBus
        /// </summary>
        /// <param name="services"></param>
        private void RegisterEventBus(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }



    }
}
