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
using IntegrationEvents.EventHandlers;
using IntegrationEvents.Events;

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

            services.AddLogging();

            //var contextOptions = new DbContextOptionsBuilder<EfContext>().UseSqlServer(Configuration.GetConnectionString("User.API.DbConnStr")).Options;

            //services.AddSingleton(contextOptions).AddScoped<EfContext>();
            //services.AddDbContext<EfContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("User.API.DbConnStr")));
            services.AddDbContext<EfContext>(opt => opt.UseSqlServer("Data Source=.;Initial Catalog=Microservice;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123456"));

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var _logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersisterConnection>>();
                var factory = new ConnectionFactory() { HostName = "localhost" };

                return new DefaultRabbitMQPersisterConnection(factory, _logger);
            });

            //RegisterEventBus(services);

            services.AddTransient<IUserRepository, UserRepository>();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            // 通过serviceProvider得到Autofac容器并获取相关服务
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Startup>();

            logger.LogInformation("Configuring services...");

            return serviceProvider;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Startup>();

            logger.LogInformation("Configuring...");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

          //  ConfigureEventBus(app);
        }


        /// <summary>
        /// 注册EventBus
        /// </summary>
        /// <param name="services"></param>
        private void RegisterEventBus(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            //   services.AddTransient<TestEventHandler>();
            //  services.AddTransient<TestEvent2Handler>();
        }


        protected virtual void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
          
        }



    }
}
