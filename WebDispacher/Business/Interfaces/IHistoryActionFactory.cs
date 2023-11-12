using DaoModels.DAO.Models;
using System.Collections.Generic;
using System;
using WebDispacher.Resources.ViewModels.History;

namespace WebDispacher.Business.Interfaces
{
    public interface IHistoryActionFactory<T,E>
    {
        void CompareAndSaveUpdatedFields(object origianl, object updated, List<T> historyEntries, int postId, HistoryActionData actionData, DateTime dateTimeUpdate, string prefix = "");
    }
}