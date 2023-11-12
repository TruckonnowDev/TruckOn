using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using System.Collections.Generic;
using System;
using WebDispacher.Attributes;
using WebDispacher.Business.Interfaces;
using WebDispacher.Resources.ViewModels.History;
using WebDispacher.ViewModels.Truck;
using WebDispacher.ViewModels.Driver;

namespace WebDispacher.Business.Services.HistoryFactory
{
    public class HistoryDriverActionFactory : IHistoryActionFactory<HistoryDriverAction, EditDriverViewModel>
    {
        public void CompareAndSaveUpdatedFields(object original, object updated, List<HistoryDriverAction> historyEntries, int driverId, HistoryActionData actionData, DateTime dateTimeUpdate, string prefix = "")
        {
            var originalType = original.GetType();

            var properties = originalType.GetProperties();

            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(HistoryAttribute)))
                {
                    object originalValue = property.GetValue(original);
                    object updatedValue = property.GetValue(updated);

                    originalValue ??= string.Empty;
                    if (!Equals(originalValue, updatedValue) && !(string.IsNullOrEmpty(originalValue.ToString()) && updatedValue == null))
                    {
                        string fieldName = prefix + property.Name;

                        historyEntries.Add(new HistoryDriverAction
                        {
                            DriverId = driverId,
                            ChangedField = fieldName,
                            ContentBefore = originalValue?.ToString() ?? string.Empty,
                            ContentAfter = updatedValue?.ToString() ?? string.Empty,
                            DateTimeAction = dateTimeUpdate,
                            ActionType = ActionType.Update,
                            UserAgent = actionData.UserAgent,
                            IpAddress = actionData.IPAddress,
                            UserId = actionData.UserId,
                            AuthorId = actionData.AuthorId,
                        });

                        property.SetValue(original, updatedValue);
                    }
                }

                if (property.PropertyType.IsClass && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                    var originalNested = property.GetValue(original);
                    var updatedNested = property.GetValue(updated);

                    if (originalNested != null && updatedNested != null)
                    {
                        CompareAndSaveUpdatedFields(originalNested, updatedNested, historyEntries, driverId, actionData, dateTimeUpdate, property.Name + " ");
                    }
                }
            }
        }
    }
}