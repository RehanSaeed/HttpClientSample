namespace HttpClientSample.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class UserAgentDelegatingHandler : DelegatingHandler
    {
        public UserAgentDelegatingHandler()
            : this(Assembly.GetEntryAssembly())
        {
        }

        public UserAgentDelegatingHandler(Assembly assembly)
            : this(GetProduct(assembly), GetVersion(assembly))
        {
        }

        public UserAgentDelegatingHandler(string applicationName, string applicationVersion)
        {
            if (applicationName == null)
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (applicationVersion == null)
            {
                throw new ArgumentNullException(nameof(applicationVersion));
            }

            this.UserAgentValues = new List<ProductInfoHeaderValue>()
            {
                new ProductInfoHeaderValue(applicationName.Replace(' ', '-'), applicationVersion),
                new ProductInfoHeaderValue($"({Environment.OSVersion})"),
            };
        }

        public UserAgentDelegatingHandler(List<ProductInfoHeaderValue> userAgentValues) =>
            this.UserAgentValues = userAgentValues ?? throw new ArgumentNullException(nameof(userAgentValues));

        public List<ProductInfoHeaderValue> UserAgentValues { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.UserAgent.Any())
            {
                foreach (var userAgentValue in this.UserAgentValues)
                {
                    request.Headers.UserAgent.Add(userAgentValue);
                }
            }

            // Else the header has already been added due to a retry.

            return base.SendAsync(request, cancellationToken);
        }

        private static string GetProduct(Assembly assembly) =>
            assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;

        private static string GetVersion(Assembly assembly) =>
            assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }
}
