using Library.Domain.Core.Messaging;
using Library.Infrastructure.DataPersistence.Rental.SQLServer;
using Library.Infrastructure.InjectionFramework;
using Library.Infrastructure.Messaging.RabbitMQ;
using Library.Infrastructure.Messaging.SignalR;
using Library.Service.Rental.Domain.DataAccessors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Service.Rental
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            InjectService();

            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://localhost:5004";
                options.RequireHttpsMetadata = false;
                options.ApiName = "rentalService";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseMvc(r =>
            {
                r.MapRoute("default", "api/{controller}/{id?}");
            });
        }

        private void InjectService()
        {
            InjectContainer.RegisterType<IRabbitMQUrlProvider, AppsettingRabbitMQUrlProvider>();
            InjectContainer.RegisterType<ICommandPublisher, RabbitMQCommandPublisher>();
            InjectContainer.RegisterType<IRentalReadDBConnectionStringProvider, AppsettingRentalReadDBConnectionStringProvider>();
            InjectContainer.RegisterType<IRentalWriteDBConnectionStringProvider, AppsettingRentalWriteDBConnectionStringProvider>();
            InjectContainer.RegisterType<IRentalReportDataAccessor, RentalReportDataAccessor>();
            InjectContainer.RegisterType<ICommandTracker, SignalRCommandTracker>();
        }
    }
}