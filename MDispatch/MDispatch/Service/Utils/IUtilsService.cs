using System.Threading.Tasks;

namespace MDispatch.Service.Utils
{
    public interface IUtilsService
    {
        Task StartListening(bool isTwoConection = false);

        void CheckNet(bool isInspection = false, bool isQueue = false);

        void RequestGPS(string longitude, string latitude);

        Task StopListening();
    }
}
