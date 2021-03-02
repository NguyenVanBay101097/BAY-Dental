﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ProductRequestService : BaseService<ProductRequest>, IProductRequestService
    {
        private readonly IMapper _mapper;
        public ProductRequestService(IAsyncRepository<ProductRequest> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ProductRequestBasic>> GetPagedResultAsync(ProductRequestPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            if (val.SaleOrderId.HasValue)
                query = query.Where(x => x.SaleOrderId == val.SaleOrderId);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateOrderTo);
            }


            var totalItems = await query.CountAsync();

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<ProductRequestBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ProductRequestBasic>>(items)
            };

            return paged;
        }

        public async Task<ProductRequestDisplay> DefaultGet(ProductRequestDefaultGet val)
        {
            var userObj = GetService<IUserService>();
            var user = await userObj.GetByIdAsync(UserId);
            var request = new ProductRequestDisplay();
            request.UserId = UserId;
            request.User = _mapper.Map<ApplicationUserSimple>(user);
            request.SaleOrderId = val.SaleOrderId;
            request.Date = DateTime.Now;
            request.State = "draft";
            return request;
        }

        public async Task<ProductRequestDisplay> GetDisplay(Guid id)
        {
            var res = await SearchQuery(x => x.Id == id)
                .Include(x => x.User)
                .Include(x => x.Employee)
                .Include(x => x.Picking)
                .Include(x => x.SaleOrder)
                .Include(x => x.Lines)
                .FirstOrDefaultAsync();

            var display = _mapper.Map<ProductRequestDisplay>(res);

            return display;
        }

        public async Task<ProductRequest> CreateRequest(ProductRequestSave val)
        {
            var prequest = _mapper.Map<ProductRequest>(val);
            SaveReuqestLines(val, prequest);
            await CreateAsync(prequest);

            return prequest;
        }

        public async override Task<ProductRequest> CreateAsync(ProductRequest entity)
        {
            var sequenceService = GetService<IIRSequenceService>();
            entity.Name = await sequenceService.NextByCode("product.request");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await _InsertProductRequestSequence();
                entity.Name = await sequenceService.NextByCode("product.request");
            }

            await base.CreateAsync(entity);

            return entity;
           
        }

        private async Task _InsertProductRequestSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu yêu cầu vật tư",
                Code = "product.request",
                Prefix = "YCVT",
                Padding = 5
            });
        }

        public async Task UpdateRequest(Guid id, ProductRequestSave val)
        {
            var medicineOrder = await SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();

            medicineOrder = _mapper.Map(val, medicineOrder);

            SaveReuqestLines(val, medicineOrder);

            await UpdateAsync(medicineOrder);

        }

        private void SaveReuqestLines(ProductRequestSave val, ProductRequest request)
        {
            var lineToRemoves = new List<ProductRequestLine>();

            foreach (var existLine in request.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                request.Lines.Remove(line);
            }

            int sequence = 0;
            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<ProductRequestLine>(line);
                    item.Sequence = sequence++;
                    request.Lines.Add(item);
                }
                else
                {
                    var item = request.Lines.SingleOrDefault(c => c.Id == line.Id);
                    if (item != null)
                    {
                        _mapper.Map(line, item);
                        item.Sequence = sequence++;
                    }
                }

            }
        }
    }
}
