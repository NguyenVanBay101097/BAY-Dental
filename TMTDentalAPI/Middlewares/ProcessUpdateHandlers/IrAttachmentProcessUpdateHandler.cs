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
        private const string _version = "1.0.1.9";
        private IServiceScopeFactory _serviceScopeFactory;


        public IrAttachmentProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
           
        }

        public async Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant != null)
                {
                    Version version1 = new Version(_version);
                    Version version2 = new Version(tenant.Version);
                    if (version2.CompareTo(version1) >= 0)
                        return;
                }

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                //chuyển dữ liệu bảng partnerimage => irattachment
                var pnImgs = context.PartnerImages.ToList();
                if(pnImgs.Any())
                {
                    var atts = new List<IrAttachment>();
                    var modelDataObj = scopedServices.GetService<IIRModelDataService>();
                    var attObj = scopedServices.GetService<IIrAttachmentService>();
                    var partnerRule = await modelDataObj.GetRef<IRRule>("base.res_partner_rule");
                    var pnIds = pnImgs.Select(x => x.PartnerId);
                    var dotkhamIds = pnImgs.Select(x => x.DotkhamId);

                    var partners = context.Partners.Where(x => pnIds.Any(z => z == x.Id)).ToList();
                    var dotkhams = context.DotKhams.Where(x => dotkhamIds.Any(z => z == x.Id)).ToList();

                    foreach (var img in pnImgs)
                    {
                        var partner = partners.FirstOrDefault(x=> x.Id == img.PartnerId);
                        var dotkham = dotkhams.FirstOrDefault(x=> x.Id == img.DotkhamId);
                        var companyId = img.PartnerId.HasValue ? (partnerRule.Active ? null : partner.CompanyId ) :
                            (img.DotkhamId.HasValue ? dotkham.CompanyId : (Guid?)null) ;
                       
                        atts.Add(new IrAttachment()
                        {
                            ResModel = img.PartnerId.HasValue ? "partner" : (img.DotkhamId.HasValue ? "dot.kham" : null),
                            ResId = img.PartnerId ?? img.DotkhamId,
                            Name = img.Name,
                            Type = "upload",
                            Url = img.UploadId,
                            CompanyId = companyId,
                            DateCreated = img.Date,
                            CreatedById = img.CreatedById,
                            WriteById = img.WriteById
                        });
                    }
                    await attObj.CreateAsync(atts);
                    context.PartnerImages.RemoveRange(pnImgs);
                    context.SaveChanges();
                }    
            }

        }
    }
}
