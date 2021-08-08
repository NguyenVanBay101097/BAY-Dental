using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class CustomerReceiptService : BaseService<CustomerReceipt>, ICustomerReceiptService
    {
        private readonly IMapper _mapper;
        public CustomerReceiptService(IAsyncRepository<CustomerReceipt> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<CustomerReceiptBasic>> GetPagedResultAsync(CustomerReceiptPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Doctor.Name.Contains(val.Search)
               || x.Partner.Name.Contains(val.Search) || x.Partner.Phone.Contains(val.Search));


            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateWaiting >= val.DateFrom.Value.AbsoluteBeginOfDate());


            if (val.DateFrom.HasValue)
            {
                var datetimeTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateWaiting <= datetimeTo);
            }

            if (val.DoctorId.HasValue)
                query = query.Where(x => x.DoctorId == val.DoctorId);

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Include(x => x.Doctor).Include(x => x.Partner).ToListAsync();

            var paged = new PagedResult2<CustomerReceiptBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<CustomerReceiptBasic>>(items)
            };

            return paged;
        }

        public async Task<CustomerReceipt> GetDisplayById(Guid id)
        {
            var customerReceipt = await SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.Doctor)
                .Include(x => x.CustomerReceiptProductRels).ThenInclude(s => s.Product)
                .FirstOrDefaultAsync();

            return customerReceipt;
        }

        public async Task<CustomerReceipt> CreateCustomerReceipt(CustomerReceiptSave val)
        {
            var customerReceipt = _mapper.Map<CustomerReceipt>(val);
            if (val.Products.Any())
            {
                foreach (var product in val.Products)
                {
                    customerReceipt.CustomerReceiptProductRels.Add(new CustomerReceiptProductRel()
                    {
                        ProductId = product.Id
                    });
                }
            }

            await CreateAsync(customerReceipt);

            return customerReceipt;
        }

        public async Task UpdateCustomerReceipt(Guid id, CustomerReceiptSave val)
        {
            var customerReceipt = await SearchQuery(x => x.Id == id)
                .Include(x => x.CustomerReceiptProductRels)
                .ThenInclude(x => x.Product).FirstOrDefaultAsync();

            customerReceipt = _mapper.Map(val, customerReceipt);

            // Xóa danh sách dịch vụ 
            customerReceipt.CustomerReceiptProductRels.Clear();


            if (val.Products.Any())
            {
                foreach (var product in val.Products)
                {
                    customerReceipt.CustomerReceiptProductRels.Add(new CustomerReceiptProductRel()
                    {
                        ProductId = product.Id
                    });
                }
            }

            await UpdateAsync(customerReceipt);
        }

        public override Task UpdateAsync(CustomerReceipt entity)
        {
            if (entity.State == "done" && !entity.DateExamination.HasValue)
                entity.DateExamination = entity.DateDone;

            return base.UpdateAsync(entity);
        }

        public async Task<long> GetCountToday(CustomerReceiptGetCountVM val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Doctor.Name.Contains(val.Search)
               || x.Partner.Name.Contains(val.Search) || x.Partner.Phone.Contains(val.Search));


            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateWaiting >= val.DateFrom.Value.AbsoluteBeginOfDate());


            if (val.DateFrom.HasValue)
            {
                var datetimeTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateWaiting <= datetimeTo);
            }

            if (val.DoctorId.HasValue)
                query = query.Where(x => x.DoctorId == val.DoctorId);

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            return await query.LongCountAsync();
        }



        public override ISpecification<CustomerReceipt> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "base.customer_receipt_comp_rule":
                    return new InitialSpecification<CustomerReceipt>(x => x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }



    }
}
