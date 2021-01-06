using ApplicationCore.Entities;
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
    public class MedicineOrderService : BaseService<MedicineOrder>, IMedicineOrderService
    {
        private readonly IMapper _mapper;
        public MedicineOrderService(IAsyncRepository<MedicineOrder> repository, IHttpContextAccessor httpContextAccessor, IMapper mappner)
            : base(repository, httpContextAccessor)
        {
            _mapper = mappner;
        }


        public async Task<PagedResult2<MedicineOrderBasic>> GetPagedResultAsync(MedicineOrderPaged val)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                   x.Partner.Name.Contains(val.Search));

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.OrderDate >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.OrderDate <= dateOrderTo);
            }


            var totalItems = await query.CountAsync();

            query = query.Include(x => x.Partner).Include(x => x.Employee).Include(x => x.ToaThuoc);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<MedicineOrderBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<MedicineOrderBasic>>(items)
            };
            return paged;
        }

        public async Task<MedicineOrderDisplay> GetByIdDisplay(Guid id)
        {
            var medicineOrder = await SearchQuery(x => x.Id == id)
                .Include(x => x.MedicineOrderLines)
                .Include(x => x.AccountPayment)
                .Include(x => x.Company)
                .Include(x => x.Employee)
                .Include(x => x.Partner)
                .Include(x => x.ToaThuoc)
                .Include(x => x.Journal)         
                .FirstOrDefaultAsync();

            var medicineOrderLineObj = GetService<IMedicineOrderLineService>();
            medicineOrder.MedicineOrderLines = await medicineOrderLineObj.SearchQuery(x => x.MedicineOrderId == medicineOrder.Id).Include(x => x.ToaThuocLine).ThenInclude(s => s.Product).ToListAsync();
            var display = _mapper.Map<MedicineOrderDisplay>(medicineOrder);

            return display;
        }

        public async Task<MedicineOrderDisplay> DefaultGet(DefaultGet val)
        {
            var toathuocObj = GetService<IToaThuocService>();
            var toathuoLinecObj = GetService<IToaThuocLineService>();
            var toathuoc = await toathuocObj.SearchQuery(x => x.Id == val.ToaThuocId).Include(x => x.Employee).Include(x => x.Partner).Include(x => x.Lines).FirstOrDefaultAsync();
            if (toathuoc == null)
                throw new Exception("Toa thuốc không tồn tại ");

            var journalObj = GetService<IAccountJournalService>();
            var journal = await journalObj.SearchQuery(x => x.Type == "cash" && x.CompanyId == CompanyId).FirstOrDefaultAsync();

            var medicineOrder = new MedicineOrderDisplay()
            {
                DateOrder = DateTime.Now,
                EmployeeId = toathuoc.EmployeeId.HasValue ? toathuoc.EmployeeId : null,
                Employee = toathuoc.EmployeeId.HasValue ? _mapper.Map<EmployeeSimple>(toathuoc.Employee) : null,
                PartnerId = toathuoc.PartnerId,
                Partner = _mapper.Map<PartnerBasic>(toathuoc.Partner),
                State = "draft",
                ToaThuocId = toathuoc.Id,
                ToaThuoc = _mapper.Map<ToaThuocDisplay>(toathuoc),
                CompanyId = CompanyId,
                Journal = _mapper.Map<AccountJournalSimple>(journal),
            };

            medicineOrder.ToaThuoc.Lines = _mapper.Map<IEnumerable<ToaThuocLineDisplay>>(await toathuoLinecObj.SearchQuery(x => x.ToaThuocId == toathuoc.Id).Include(x => x.Product).ToListAsync());

            return medicineOrder;
        }

        public async Task<MedicineOrder> CreateMedicineOrder(MedicineOrderSave val)
        {
            var medicineOrder = _mapper.Map<MedicineOrder>(val);
            SaveOrderLines(val, medicineOrder);
            await CreateAsync(medicineOrder);

            return medicineOrder;
        }



        public async Task UpdateMedicineOrder(Guid id, MedicineOrderSave val)
        {
            var medicineOrder = await SearchQuery(x => x.Id == id).Include(x => x.MedicineOrderLines).FirstOrDefaultAsync();

            medicineOrder = _mapper.Map(val, medicineOrder);

            SaveOrderLines(val, medicineOrder);

            await UpdateAsync(medicineOrder);

        }

        public async Task ActionPayment(IEnumerable<Guid> id)
        {

        }

        public async Task ActionCancel(IEnumerable<Guid> id)
        {

        }



        private void SaveOrderLines(MedicineOrderSave val, MedicineOrder order)
        {
            var lineToRemoves = new List<MedicineOrderLine>();

            foreach (var existLine in order.MedicineOrderLines)
            {
                if (!val.MedicineOrderLines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                order.MedicineOrderLines.Remove(line);
            }

            foreach (var line in val.MedicineOrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var item = _mapper.Map<MedicineOrderLine>(line);
                    order.MedicineOrderLines.Add(item);
                }
                else
                {
                    var l = order.MedicineOrderLines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                }

            }
        }

        public async override Task<MedicineOrder> CreateAsync(MedicineOrder entity)
        {
            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("medicine.order");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await InsertMedicineOrderSequence();
                entity.Name = await sequenceService.NextByCode("medicine.order");
            }

            await base.CreateAsync(entity);
            return entity;
        }

        private async Task InsertMedicineOrderSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "medicine.order",
                Name = "Mã hóa đơn thuốc",
                Prefix = "HDTT/{dd}{MM}{yy}-",
                Padding = 5,
            });
        }
    }
}
