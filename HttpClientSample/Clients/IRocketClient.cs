namespace HttpClientSample.Clients
{
    using System.Threading.Tasks;
    using HttpClientSample.Models;

    public interface IRocketClient
    {
        Task<TakeoffStatus> GetStatus(bool working);
    }
}
