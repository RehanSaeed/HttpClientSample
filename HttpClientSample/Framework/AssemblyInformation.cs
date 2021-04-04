namespace HttpClientSample
{
    using System.Reflection;

    public record AssemblyInformation(string Product, string Version)
    {
        public static readonly AssemblyInformation Current = new(typeof(AssemblyInformation).Assembly);

        public AssemblyInformation(Assembly assembly)
            : this(
                assembly.GetCustomAttribute<AssemblyProductAttribute>().Product,
                assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version)
        {
        }
    }
}
