﻿using ApplicationCore.Entities;
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

        public async Task<DotKham> GetDotKhamForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x=>x.Doctor)
                .Include(x => x.User)
                .Include(x => x.SaleOrder)
                .Include(x => x.Lines)
                .Include(x => x.Appointment)
                .Include("Lines.Product")
                .Include("Lines.Operations")
                .Include("Lines.Operations.Product")
                .FirstOrDefaultAsync();
        }

        public async Task<DotKhamDisplay> GetDotKhamDisplayAsync(Guid id)
        {
            return await _mapper.ProjectTo<DotKhamDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            //foreach (var dk in self)
            //{
            //    if (dk.Steps.Any(x => x.IsDone))
            //        throw new Exception("Không thể xóa đợt khám đã có công đoạn hoàn thành");
            //}

            await DeleteAsync(self);
        }

        public async Task<IEnumerable<DotKham>> GetDotKhamsForInvoice(Guid invoiceId)
        {
            return await SearchQuery(x => x.InvoiceId == invoiceId, orderBy: x => x.OrderByDescending(s => s.DateCreated))
                .Include(x => x.User)
                .Include(x=>x.Doctor)
                .ToListAsync();
        }

        public async Task<IEnumerable<DotKham>> GetDotKhamsForSaleOrder(Guid saleOrderId)
        {
            return await SearchQuery(x => x.SaleOrderId == saleOrderId, orderBy: x => x.OrderByDescending(s => s.DateCreated))
                .Include(x => x.User)
                .Include(x => x.Doctor)
                .ToListAsync();
        }

        public async Task CreateOrUpdateDotKham(DotKhamVm val)
        {
            var dotKham = _mapper.Map<DotKham>(val);

            SaveLines(val, dotKham);

            SaveDotKhamImages(val, dotKham);

            if (val.Id == Guid.Empty)
            {
                await CreateAsync(dotKham);
            }
            else
            {
                await UpdateAsync(dotKham);
            }

        }

        private void SaveLines(DotKhamVm val, DotKham dotkham)
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
                line.Sequence = sequence++;

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    dotkham.Lines.Add(_mapper.Map<DotKhamLine>(line));
                }
                else
                {
                    _mapper.Map(line, dotkham.Lines.SingleOrDefault(c => c.Id == line.Id));
                }
            }

        }

        private void SaveDotKhamImages(DotKhamVm val, DotKham dotkham)
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
                    dotkham.DotKhamImages.Add(_mapper.Map<PartnerImage>(img));
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
            entity.Name = entity.Name;
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                entity.Name = "Đợt khám";
                //await InsertDotKhamSequence();
                //entity.Name = await sequenceService.NextByCode("dot.kham");
            }

            //if (entity.SaleOrderId.HasValue)
            //{
            //    var saleObj = GetService<ISaleOrderService>();
            //    var order = await saleObj.GetByIdAsync(entity.SaleOrderId.Value);
            //    entity.PartnerId = order.PartnerId;
            //}

            await base.CreateAsync(entity);
            return entity;
        }

        public async Task ActionConfirm(Guid id)
        {
            var dotKham = await SearchQuery(x => x.Id == id)
                .Include(x => x.Invoice)
                .Include("Invoice.InvoiceLines")
                .FirstOrDefaultAsync();
            var routingObj = GetService<IRoutingService>();
            var dotKhamStepObj = GetService<IDotKhamStepService>();
            var productStepObj = GetService<IProductStepService>();
            var dotKhamSteps = new List<DotKhamStep>();

            dotKham.State = "confirmed";
            await UpdateAsync(dotKham);            
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

            var items = await _mapper.ProjectTo<DotKhamBasic>(query.OrderByDescending(x => x.Date).Skip(val.Offset).Take(val.Limit)) 
                .ToListAsync();
            var totalItems = await query.CountAsync();

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
