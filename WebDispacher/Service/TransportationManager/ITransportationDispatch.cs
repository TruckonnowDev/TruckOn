using DaoModels.DAO.Models;
using System.Threading.Tasks;

namespace WebDispacher.Service.TransportationManager
{
    public interface ITransportationDispatch
    {
        Task<Shipping> GetShipping(string urlPage, Dispatcher dispatcher);
    }
}