using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using WebDispacher.Constants;
using WebDispacher.ViewModels.Order;

namespace WebDispacher.Business.Extensions
{
    public static class OrderExtensions
    {
        public static async Task UpdateOrderWithHistory<T>(this T order, Context context, Order model, string localDate, params string[] propertiesToTrack)
        {
            var dateTimeUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var historyOrderActions = new List<HistoryOrderAction>();

            foreach (var propertyName in propertiesToTrack)
            {
                var propertyInfo = typeof(T).GetProperty(propertyName);
                var originalValue = propertyInfo.GetValue(order);
                var newValue = propertyInfo.GetValue(model);

                if (!Equals(originalValue, newValue))
                {
                    propertyInfo.SetValue(order, newValue);

                    var historyOrderAction = new HistoryOrderAction
                    {
                        OrderId = (int)propertyInfo.GetValue(order, null),
                        ActionType = ActionType.Update,
                        FieldAction = propertyName,
                        ContentFrom = originalValue?.ToString() ?? string.Empty,
                        ContentTo = newValue?.ToString() ?? string.Empty,
                        DateTimeAction = dateTimeUpdate
                    };

                    historyOrderActions.Add(historyOrderAction);
                }
            }

            if (historyOrderActions.Any())
            {
                await context.HistoriesOrdersActions.AddRangeAsync(historyOrderActions);
                await context.SaveChangesAsync();
            }
        }

        // Utility method to get the property name from an expression
        private static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
                return memberExpression.Member.Name;
            if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
                return operand.Member.Name;
            throw new ArgumentException("Invalid expression");
        }

        // Utility method to get the key values of an entity
        private static object[] GetKeyValues<T>(T entity) where T : class
        {
            var entityType = typeof(T);
            var keyProperties = entityType.GetProperties().Where(p => p.GetCustomAttribute<KeyAttribute>() != null).ToArray();
            var keyValues = keyProperties.Select(p => p.GetValue(entity)).ToArray();
            return keyValues;
        }

        // Utility method to get the value of a property from an entity
        private static object GetPropertyValue<T>(T entity, string propertyName)
        {
            var entityType = typeof(T);
            var property = entityType.GetProperty(propertyName);
            return property?.GetValue(entity);
        }

        // Utility method to set the value of a property on an entity
        private static void SetPropertyValue<T>(T entity, string propertyName, object value)
        {
            var entityType = typeof(T);
            var property = entityType.GetProperty(propertyName);
            property?.SetValue(entity, value);
        }

        // Utility method to compare two values for equality
        private static bool AreEqual(object value1, object value2)
        {
            if (value1 == null && value2 == null)
                return true;
            if (value1 == null || value2 == null)
                return false;
            return value1.Equals(value2);
        }
    }
}
