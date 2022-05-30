using System.Threading.Tasks;

namespace MDispatch.Service.GoogleApi
{
    public interface IGoogleApiService
    {
        Task<int> DetectPlate(string token, string image, string idDriver, string type, string plate);
    }
}
