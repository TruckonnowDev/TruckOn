using System.Threading.Tasks;

namespace MDispatch.Service.RequestQueue
{
    public interface IManagerQueueService
    {
        Task AddRequest(string nameRequvest, string token, params dynamic[] param);
    }
}
