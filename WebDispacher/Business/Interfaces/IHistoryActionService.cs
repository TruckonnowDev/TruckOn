using System.Threading.Tasks;
using System;
using WebDispacher.Resources.ViewModels.History;

namespace WebDispacher.Business.Interfaces
{
    public interface IHistoryActionService
    {
        Task CompareAndSaveUpdatedFields<T, E>(E original, E updated, int id, HistoryActionData actionData, DateTime dateTimeUpdate)
            where T : class;
    }
}