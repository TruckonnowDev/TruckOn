using DaoModels.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WebDispacher.Business.Interfaces;
using WebDispacher.Resources.ViewModels.History;

namespace WebDispacher.Business.Services
{
    public class HistoryActionService : IHistoryActionService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICompanyService companyService;

        public HistoryActionService(
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            ICompanyService companyService)
        {
            this.serviceProvider = serviceProvider;
            this.httpContextAccessor = httpContextAccessor;
            this.companyService = companyService;
        }

        public async Task CompareAndSaveUpdatedFields<T,E>(E original, E updated, int id, HistoryActionData actionData, DateTime dateTimeUpdate) 
            where T : class
        {
            var factory = serviceProvider.GetService<IHistoryActionFactory<T,E>>();

            if (factory == null)
            {
                throw new InvalidOperationException($"Factory for {typeof(T).Name} not registered.");
            }

            var historyEntries = new List<T>();

            factory.CompareAndSaveUpdatedFields(original, updated, historyEntries, id, actionData, dateTimeUpdate);

            using (var db = new Context())
            {
                await db.Set<T>().AddRangeAsync(historyEntries);

                await db.SaveChangesAsync();
            }
        }
    }
}