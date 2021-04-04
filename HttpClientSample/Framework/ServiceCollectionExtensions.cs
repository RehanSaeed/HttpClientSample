namespace HttpClientSample.Framework
{
    using System;
    using HttpClientSample.Options;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Polly;
    using Polly.Extensions.Http;

    public static class ServiceCollectionExtensions
    {
        private const string PoliciesConfigurationSectionName = "Policies";

        public static IServiceCollection AddPolicies(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName = PoliciesConfigurationSectionName)
        {
            services.Configure<PolicyOptions>(configuration);
            var policyOptions = configuration.GetSection(configurationSectionName).Get<PolicyOptions>();

            var policyRegistry = services.AddPolicyRegistry();
            policyRegistry.Add(
                PolicyName.HttpRetry,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
            policyRegistry.Add(
                PolicyName.HttpCircuitBreaker,
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }

        public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName)
            where TClient : class
            where TImplementation : class, TClient
            where TClientOptions : HttpClientOptions, new() =>
            services
                .Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
                .AddSingleton<CorrelationIdDelegatingHandler>()
                .AddSingleton<UserAgentDelegatingHandler>()
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient(
                    (serviceProvider, httpClient) =>
                    {
                        var httpClientOptions = serviceProvider
                            .GetRequiredService<IOptions<TClientOptions>>()
                            .Value;
                        httpClient.BaseAddress = httpClientOptions.BaseAddress;
                        httpClient.Timeout = httpClientOptions.Timeout;
                    })
                .ConfigurePrimaryHttpMessageHandler(x => new DefaultHttpClientHandler())
                .AddPolicyHandlerFromRegistry(PolicyName.HttpRetry)
                .AddPolicyHandlerFromRegistry(PolicyName.HttpCircuitBreaker)
                .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
                .AddHttpMessageHandler<UserAgentDelegatingHandler>()
                .Services;
    }
}
