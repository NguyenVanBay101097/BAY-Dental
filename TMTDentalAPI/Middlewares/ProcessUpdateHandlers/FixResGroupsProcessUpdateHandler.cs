using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class FixResGroupsProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.1";
        private IServiceScopeFactory _serviceScopeFactory;

        public FixResGroupsProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant != null)
                {
                    //Version version1 = new Version(_version);
                    //Version version2 = new Version(tenant.Version);
                    //if (version2.CompareTo(version1) >= 0)
                    //    return Task.CompletedTask;
                }

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                var myCache = scope.ServiceProvider.GetService<IMyCache>();

                var modelDatas = context.IRModelDatas.Where(x => x.Model == "res.groups")
                    .OrderBy(x => x.DateCreated)
                    .ToList();

                var modelDataDict = new Dictionary<string, IRModelData>();
                var modelDataToRemoves = new List<IRModelData>();
                var resGroupToRemoves = new List<ResGroup>();
                foreach(var modelData in modelDatas)
                {
                    var id = $"{modelData.Module}.{modelData.Name}";
                    if (!modelDataDict.ContainsKey(id))
                        modelDataDict.Add(id, modelData);
                    else
                        modelDataToRemoves.Add(modelData);
                }

                var groupIds = modelDataToRemoves.Select(x => new Guid(x.ResId)).Distinct().ToList();
                var groups = context.ResGroups.Where(x => groupIds.Contains(x.Id)).ToList();
                context.RemoveRange(groups);
                context.SaveChanges();

                context.RemoveRange(modelDataToRemoves);
                context.SaveChanges();

                var pattern = $"{(tenant != null ? tenant.Hostname : "localhost")}-irmodeldata";
                myCache.RemoveByPattern(pattern);
            }

            return Task.CompletedTask;
        }
    }
}

