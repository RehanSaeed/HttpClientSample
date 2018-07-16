namespace HttpClientSample.Framework
{
    using System.Net;
    using System.Net.Http;

    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler() =>
            this.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    }
}
