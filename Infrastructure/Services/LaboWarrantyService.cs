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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class LaboWarrantyService : BaseService<LaboWarranty>, ILaboWarrantyService
    {
        private readonly IMapper _mapper;
        public LaboWarrantyService(
            IAsyncRepository<LaboWarranty> repository, 
            IHttpContextAccessor httpContextAccessor, 
            IUploadService uploadService,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<LaboWarrantyBasic>> GetPagedResultAsync(LaboWarrantyPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                                x.LaboOrder.Customer.Name.Contains(val.Search) ||
                                x.LaboOrder.Name.Contains(val.Search));

            if (val.SupplierId.HasValue)
            {
                query = query.Where(x => x.LaboOrder.PartnerId == val.SupplierId);
            }

            if (val.LaboOrderId.HasValue)
            {
                query = query.Where(x => x.LaboOrderId == val.LaboOrderId);
            }

            if (val.DateReceiptFrom.HasValue)
            {
                var dateFrom = val.DateReceiptFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.DateReceiptWarranty >= val.DateReceiptFrom);
            }

            if (val.DateReceiptTo.HasValue)
            {
                var dateTo = val.DateReceiptTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateReceiptWarranty <= val.DateReceiptTo);
            }

            if (!string.IsNullOrEmpty(val.States))
            {
                var states = val.States.Split(",");
                query = query.Where(x => states.Contains(x.State));
            }

            if (val.NotDraft.HasValue && val.NotDraft == true)
            {
                query = query.Where(x => x.State != "draft");
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.LaboOrder).Include(x => x.LaboOrder.Customer);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<LaboWarrantyBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<LaboWarrantyBasic>>(items)
            };

            return paged;
        }

        public async Task<LaboWarrantyDisplay> GetDefault(LaboWarrantyGetDefault val)
        {
            var res = new LaboWarrantyDisplay();

            if (val.LaboOrderId.HasValue)
            {
                var laboOrderObj = GetService<ILaboOrderService>();
                var laboOrder = await laboOrderObj.SearchQuery(x => x.Id == val.LaboOrderId)
                    .Include(x => x.Partner)
                    .Include(x => x.Customer)
                    .Include(x => x.SaleOrderLine)
                    .Include(x => x.SaleOrderLine.Employee)
                    .FirstOrDefaultAsync();

                var teeth = await laboOrderObj.SearchQuery(x => x.Id == val.LaboOrderId)
                    .SelectMany(x => x.LaboOrderToothRel)
                    .Select(x => x.Tooth).ToListAsync();

                res.TeethLaboOrder = _mapper.Map<IEnumerable<ToothDisplay>>(teeth);
                res.LaboOrderId = laboOrder.Id;
                res.LaboOrderName = laboOrder.Name;
                res.SupplierId = laboOrder.PartnerId;
                res.SupplierName = laboOrder.Partner.Name;
                res.CustomerId = laboOrder.CustomerId;
                res.CustomerRef = laboOrder.Customer.Ref;
                res.CustomerName = laboOrder.Customer.Name;
                res.CustomerDisplayName = laboOrder.Customer.DisplayName;
                res.SaleOrderLineName = laboOrder.SaleOrderLine.Name;
                res.EmployeeId = laboOrder.SaleOrderLine.EmployeeId;
                res.Employee = _mapper.Map<EmployeeSimple>(laboOrder.SaleOrderLine.Employee);
                res.State = "draft";
            }

            return res;
        }

        public async Task<LaboWarrantyDisplay> GetLaboWarrantyDisplay(Guid id)
        {
            var laboWarranty = await SearchQuery(x => x.Id == id)
                .Include(x => x.LaboOrder)
                .Include(x => x.LaboOrder.Partner)
                .Include(x => x.LaboOrder.Customer)
                .Include(x => x.LaboOrder.SaleOrderLine)
                .Include(x => x.Employee)
                .Include(x => x.LaboWarrantyToothRels).ThenInclude(x => x.Tooth)
                .FirstOrDefaultAsync();

            // Lấy danh sách răng của Phiếu Labo
            var res = _mapper.Map<LaboWarrantyDisplay>(laboWarranty);
            var laboOrderObj = GetService<ILaboOrderService>();
            var teeth = await laboOrderObj.SearchQuery(x => x.Id == laboWarranty.LaboOrderId)
                .SelectMany(x => x.LaboOrderToothRel)
                .Select(x => x.Tooth).ToListAsync();
            res.TeethLaboOrder = _mapper.Map<IEnumerable<ToothDisplay>>(teeth);

            // Lấy danh sách răng của Phiếu bảo hành
            res.Teeth = _mapper.Map<IEnumerable<ToothDisplay>>(laboWarranty.LaboWarrantyToothRels.Select(x => x.Tooth).ToList());

            return res;
        }

        public async Task<LaboWarranty> CreateLaboWarranty(LaboWarrantySave val)
        {
            var laboWarranty = _mapper.Map<LaboWarranty>(val);
            laboWarranty.CompanyId = CompanyId;

            // Thêm danh sách răng
            foreach (var tooth in val.Teeth)
            {
                laboWarranty.LaboWarrantyToothRels.Add(new LaboWarrantyToothRel() { ToothId = tooth.Id });
            }
            await CreateAsync(laboWarranty);

            return laboWarranty;
        }

        public override async Task<IEnumerable<LaboWarranty>> CreateAsync(IEnumerable<LaboWarranty> entities)
        {
            await _UpdateProperties(entities);
            return await base.CreateAsync(entities);
        }

        private async Task _UpdateProperties(IEnumerable<LaboWarranty> self)
        {
            foreach (var laboWarranty in self)
            {
                if (string.IsNullOrEmpty(laboWarranty.Name))
                {
                    var sequenceObj = GetService<IIRSequenceService>();
                    laboWarranty.Name = await sequenceObj.NextByCode("labo.warranty");
                    if (string.IsNullOrEmpty(laboWarranty.Name))
                    {
                        await sequenceObj.CreateAsync(new IRSequence
                        {
                            Name = "Bảo hành Labo",
                            Code = "labo.warranty",
                            Prefix = "BH",
                            Padding = 5
                        });

                        laboWarranty.Name = await sequenceObj.NextByCode("labo.warranty");
                    }
                }
            }
        }

        public async Task UpdateLaboWarranty(Guid id, LaboWarrantySave val)
        {
            var laboWarranty = await SearchQuery(x => x.Id == id)
                .Include(x => x.LaboOrder)
                .Include(x => x.LaboOrder.Partner)
                .Include(x => x.LaboOrder.Customer)
                .Include(x => x.LaboOrder.SaleOrderLine)
                .Include(x => x.Employee)
                .Include(x => x.LaboWarrantyToothRels).ThenInclude(x => x.Tooth)
                .FirstOrDefaultAsync();

            laboWarranty = _mapper.Map(val, laboWarranty);

            // Thêm danh sách răng
            laboWarranty.LaboWarrantyToothRels.Clear();
            foreach (var tooth in val.Teeth)
            {
                laboWarranty.LaboWarrantyToothRels.Add(new LaboWarrantyToothRel() { ToothId = tooth.Id });
            }

            await UpdateAsync(laboWarranty);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var laboWarranty in self)
            {
                if (laboWarranty.State != "draft")
                    throw new Exception("Chỉ có thế xóa phiếu bảo hành ở trạng thái nháp");
            }

            await DeleteAsync(self);
        }

        public async Task ButtonConfirm(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var laboWarranty in self)
            {
                if (laboWarranty.State != "draft")
                    throw new Exception("Chỉ có thể xác nhận ở trạng thái nháp");
                laboWarranty.State = "new";
            }

            await UpdateAsync(self);
        }

        public async Task ButtonCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var laboWarranty in self)
            {
                if (laboWarranty.DateSendWarranty != null)
                    throw new Exception("Không thể hủy phiếu bảo hành đã Gửi bảo hành");
                laboWarranty.State = "draft";
            }

            await UpdateAsync(self);
        }

        public async Task ConfirmSendWarranty(LaboWarrantyConfirm val)
        {
            var laboWarranty = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();
            if (laboWarranty.State != "new")
                throw new Exception("Chỉ có thể xác nhận ở trạng thái mới");
            laboWarranty.State = "sent";
            laboWarranty.DateSendWarranty = val.Date;

            await UpdateAsync(laboWarranty);
        }

        public async Task CancelSendWarranty(Guid id)
        {
            var laboWarranty = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (laboWarranty.State != "sent")
                throw new Exception("Chỉ có thể hủy ở trạng thái đã gửi");
            laboWarranty.State = "new";
            laboWarranty.DateSendWarranty = null;

            await UpdateAsync(laboWarranty);
        }

        public async Task ConfirmReceiptInspection(LaboWarrantyConfirm val)
        {
            var laboWarranty = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();
            if (laboWarranty.State != "sent")
                throw new Exception("Chỉ có thể xác nhận ở trạng thái đã gửi");
            laboWarranty.State = "received";
            laboWarranty.DateReceiptInspection = val.Date;

            await UpdateAsync(laboWarranty);
        }

        public async Task CancelReceiptInspection(Guid id)
        {
            var laboWarranty = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (laboWarranty.State != "received")
                throw new Exception("Chỉ có thể hủy ở trạng thái đã nhận");
            laboWarranty.State = "sent";
            laboWarranty.DateReceiptInspection = null;

            await UpdateAsync(laboWarranty);
        }

        public async Task ConfirmAssemblyWarranty(LaboWarrantyConfirm val)
        {
            var laboWarranty = await SearchQuery(x => x.Id == val.Id).FirstOrDefaultAsync();
            if (laboWarranty.State != "received")
                throw new Exception("Chỉ có thể xác nhận ở trạng thái đã nhận");
            laboWarranty.State = "assembled";
            laboWarranty.DateAssemblyWarranty = val.Date;

            await UpdateAsync(laboWarranty);
        }

        public async Task CancelAssemblyWarranty(Guid id)
        {
            var laboWarranty = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (laboWarranty.State != "assembled")
                throw new Exception("Chỉ có thể hủy ở trạng thái đã lắp");
            laboWarranty.State = "received";
            laboWarranty.DateAssemblyWarranty = null;

            await UpdateAsync(laboWarranty);
        }
    }
}
