using ApplicationCore.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class IrAttachmentProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification> 
    {
        private const string _version = "1.0.1.8";
        private IServiceScopeFactory _serviceScopeFactory;


        public IrAttachmentProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
           
        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant == null)
                    return Task.CompletedTask;

                Version version1 = new Version(_version);
                Version version2 = new Version(tenant.Version);
                if (version2.CompareTo(version1) >= 0)
                    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                //chuyển dữ liệu bảng partnerimage => irattachment
                var imgs = context.PartnerImages.ToList();
                if(imgs.Any())
                {
                    var atts = new List<IrAttachment>();
                    foreach (var img in imgs)
                    {
                        atts.Add( new IrAttachment()
                        {
                            ResModel = img.PartnerId != null ? "partner" : "dot.kham",
                            ResId = img.PartnerId ?? img.DotkhamId,
                            Name = img.Name,
                            Type = "upload",
                            Url = img.UploadId,
                            //CompanyId = CompanyId
                        });
                    }
                    context.AddRange(atts);
                    context.SaveChanges();
                }    
            }

            return Task.CompletedTask;
        }
    }
}
