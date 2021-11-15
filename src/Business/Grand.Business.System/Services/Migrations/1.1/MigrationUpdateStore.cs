﻿using Grand.Business.Common.Interfaces.Logging;
using Grand.Domain.Data;
using Grand.Domain.Stores;
using Grand.Infrastructure.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Grand.Business.System.Services.Migrations._1._1
{
    public class MigrationUpdateStore : IMigration
    {
        public int Priority => 0;
        public DbVersion Version => new(1, 1);
        public Guid Identity => new("31F803A6-4B54-42F5-9868-6115F51BA594");
        public string Name => "Update store - add domains";

        /// <summary>
        /// Upgrade process
        /// </summary>
        /// <param name="database"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public bool UpgradeProcess(IDatabaseContext database, IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetRequiredService<IRepository<Store>>();
            var logService = serviceProvider.GetRequiredService<ILogger>();
            try
            {
                foreach (var store in repository.Table)
                {
                    var host = new HostString(store.Url);
                    var httpscheme = store.SslEnabled ? "https" : "http";
                    var domain = new DomainHost() {
                        HostName = host.Host,
                        Url = $"{httpscheme}://{host}",
                        Primary = true
                    };
                    store.Domains.Add(domain);

                    repository.Update(store);
                }
            }
            catch (Exception ex)
            {
                logService.InsertLog(Domain.Logging.LogLevel.Error, "UpgradeProcess - MigrationUpdateStore", ex.Message).GetAwaiter().GetResult();
            }
            return true;
        }
    }
}