namespace HttpClientSample
{
    using CorrelationId;
    using CorrelationId.DependencyInjection;
    using HttpClientSample.Clients;
    using HttpClientSample.Framework;
    using HttpClientSample.Options;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration) => this.configuration = configuration;

        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddDefaultCorrelationId()
                .AddControllers()
                .Services
                .AddPolicies(this.configuration)
                .AddHttpClient<IRocketClient, RocketClient, RocketClientOptions>(
                    this.configuration,
                    nameof(ApplicationOptions.RocketClient));

        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }

            application
                .UseCorrelationId()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
