using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class DotKhamService : BaseService<DotKham>, IDotKhamService
    {
        private readonly IMapper _mapper;
        public DotKhamService(IAsyncRepository<DotKham> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<DotKhamDisplay> GetDotKhamDisplayAsync(Guid id)
        {

            var dkLineObj= GetService<IDotKhamLineService>();
            var pnImageObj = GetService<IPartnerImageService>();

            var res = _mapper.Map<DotKhamDisplay>( await SearchQuery(x => x.Id == id).Include(x=> x.Doctor).FirstOrDefaultAsync());

            if (res == null) return null;

            res.Lines = await dkLineObj.SearchQuery(x => x.DotKhamId == id).OrderBy(x=> x.Sequence).Select(x => new DotKhamLineDisplay
            {
                Teeth = x.ToothRels.Select(x => new ToothDisplay
                {
                    Id = x.ToothId,
                    Name = x.Tooth.Name
                }),
                Id = x.Id,
                NameStep = x.NameStep,
                Note = x.Note,
                Product = new ProductSimple
                {
                    Id = x.ProductId.Value,
                    Name = x.Product.Name
                },
                SaleOrderLineId = x.SaleOrderLineId
            }).ToListAsync();

            res.DotKhamImages = await _mapper.ProjectTo<PartnerImageDisplay>(pnImageObj.SearchQuery(x => x.DotkhamId.Value == id)).ToListAsync();

            return res;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var partnerImgobj = GetService<IPartnerImageService>();
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.DotKhamImages).ToListAsync();
            foreach (var item in self)
            {
                var dotKhamImages = item.DotKhamImages;
                await partnerImgobj.DeleteAsync(dotKhamImages);
            }
            //foreach (var dk in self)
            //{
            //    if (dk.Steps.Any(x => x.IsDone))
            //        throw new Exception("Không thể xóa đợt khám đã có công đoạn hoàn thành");
            //}

            await DeleteAsync(self);
        }

        public async Task<IEnumerable<DotKham>> GetDotKhamsForSaleOrder(Guid saleOrderId)
        {
            return await SearchQuery(x => x.SaleOrderId == saleOrderId, orderBy: x => x.OrderByDescending(s => s.DateCreated))
                .Include(x => x.Doctor)
                .ToListAsync();
        }

        public async Task<DotKham> CreateDotKham(Guid saleOrderId, DotKhamSaveVm val)
        {
            var orderobj = GetService<ISaleOrderService>();
            var dotKham = _mapper.Map<DotKham>(val);
            dotKham.Sequence = val.Sequence;
            dotKham.CompanyId = CompanyId;
            dotKham.SaleOrderId = saleOrderId;
            dotKham.PartnerId = await orderobj.SearchQuery(x=>x.Id == saleOrderId).Select(x=>x.PartnerId).FirstOrDefaultAsync();

            ///tạo lines
            var sequence = 0;
            foreach (var item in val.Lines)
            {
                var line = _mapper.Map<DotKhamLine>(item);
                line.Sequence = sequence++;
                foreach (var toothId in item.ToothIds)
                {
                    line.ToothRels.Add(new DotKhamLineToothRel
                    {
                        ToothId = toothId
                    });
                }

                dotKham.Lines.Add(line);
            }

            ///tạo dot kham images
            foreach (var img in val.DotKhamImages)
            {
                var image = _mapper.Map<PartnerImage>(img);
                image.PartnerId = dotKham.PartnerId;
                dotKham.DotKhamImages.Add(image);
            }

            await CreateAsync(dotKham);
            return dotKham;
            
        }

        public async Task UpdateDotKham(Guid id, DotKhamSaveVm val)
        {
            var dotKham =  await SearchQuery(x => x.Id == id)
                .Include(x=>x.DotKhamImages)
                .Include(x=>x.Lines)
                .Include("Lines.ToothRels")
                .Include("Lines.Product")
                .FirstOrDefaultAsync();

            dotKham = _mapper.Map(val, dotKham);

            SaveLines(val, dotKham);

            SaveDotKhamImages(val, dotKham);

            await UpdateAsync(dotKham);

        }

        private void SaveLines(DotKhamSaveVm val, DotKham dotkham)
        {
            //remove line
            var lineToRemoves = new List<DotKhamLine>();
            foreach (var existLine in dotkham.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                dotkham.Lines.Remove(line);
            }

            int sequence = 1;        

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<DotKhamLine>(line);       
                    item.Sequence = sequence++;
                    foreach (var toothId in line.ToothIds)
                    {
                        item.ToothRels.Add(new DotKhamLineToothRel
                        {
                            ToothId = toothId
                        });
                    }
                    dotkham.Lines.Add(item);
                }
                else
                {
                   var res = _mapper.Map(line, dotkham.Lines.SingleOrDefault(c => c.Id == line.Id));
                    if(res != null)
                    {
                        res.Sequence = sequence++;
                        res.ToothRels.Clear();
                        foreach (var toothId in line.ToothIds)
                        {
                            res.ToothRels.Add(new DotKhamLineToothRel
                            {
                                ToothId = toothId
                            });
                        }
                    }
                  
                }
            }

        }

        private void SaveDotKhamImages(DotKhamSaveVm val, DotKham dotkham)
        {
            //remove line
            var lineToRemoves = new List<PartnerImage>();
            foreach (var existLine in dotkham.DotKhamImages)
            {
                if (!val.DotKhamImages.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                dotkham.DotKhamImages.Remove(line);
            }

            foreach (var img in val.DotKhamImages)
            {
                if (img.Id == Guid.Empty)
                {
                    var image = _mapper.Map<PartnerImage>(img);
                    image.PartnerId = dotkham.PartnerId;
                    dotkham.DotKhamImages.Add(image);
                }
                else
                {
                    _mapper.Map(img, dotkham.DotKhamImages.SingleOrDefault(c => c.Id == img.Id));
                }
            }

        }



        public async Task<IEnumerable<DotKhamBasic>> GetDotKhamBasicsForSaleOrder(Guid saleOrderId)
        {
            return await _mapper.ProjectTo<DotKhamBasic>(SearchQuery(x => x.SaleOrderId == saleOrderId, orderBy: x => x.OrderByDescending(s => s.DateCreated))).ToListAsync();
        }


        public async override Task<DotKham> CreateAsync(DotKham entity)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("dot.kham");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await InsertDotKhamSequence();
                entity.Name = await sequenceService.NextByCode("dot.kham");
            }

            if (entity.SaleOrderId.HasValue)
            {
                var saleObj = GetService<ISaleOrderService>();
                var order = await saleObj.GetByIdAsync(entity.SaleOrderId.Value);
                entity.PartnerId = order.PartnerId;
            }

            await base.CreateAsync(entity);
            return entity;
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var dk in self)
                dk.State = "cancel";
            await UpdateAsync(self);
        }

        public async Task<PagedResult2<DotKhamBasic>> GetPagedResultAsync(DotKhamPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Partner.Name.Contains(val.Search) ||
                x.Partner.NameNoSign.Contains(val.Search));

            if (val.AppointmentId.HasValue)
                query = query.Where(x => x.AppointmentId.Equals(val.AppointmentId));

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)  
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await _mapper.ProjectTo<DotKhamBasic>(query.Include(x => x.Doctor).Include(z => z.SaleOrder).OrderByDescending(x => x.DateCreated).ThenByDescending(x => x.Sequence)).ToListAsync();

            return new PagedResult2<DotKhamBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<DotKhamDisplay> DefaultGet(DotKhamDefaultGet val)
        {
            var res = new DotKhamDisplay();
            res.CompanyId = CompanyId;
            res.SaleOrderId = val.SaleOrderId;
            return res;
        }

        private async Task InsertDotKhamSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "dot.kham",
                Name = "Mã đợt khám",
                Prefix = "DK",
                Padding = 6,
            });
        }

        public override ISpecification<DotKham> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.dot_kham_comp_rule":
                    return new InitialSpecification<DotKham>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
