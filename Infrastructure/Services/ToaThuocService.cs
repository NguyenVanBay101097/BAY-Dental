using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ToaThuocService : BaseService<ToaThuoc>, IToaThuocService
    {
        private readonly IMapper _mapper;
        private readonly ISamplePrescriptionService _samplePrescriptionService;
        public ToaThuocService(IAsyncRepository<ToaThuoc> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ISamplePrescriptionService samplePrescriptionService)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _samplePrescriptionService = samplePrescriptionService;
        }

        public async Task<ToaThuocDisplay> GetToaThuocForDisplayAsync(Guid id)
        {
            var toathuoc = await SearchQuery(x => x.Id == id)
                .Include(x => x.Employee)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();

            if (toathuoc == null)
                return null;

            var res = _mapper.Map<ToaThuocDisplay>(toathuoc);

            var toaThuocLineObj = GetService<IToaThuocLineService>();
            var lines = await toaThuocLineObj.SearchQuery(x => x.ToaThuocId == id, orderBy: x => x.OrderBy(s => s.Sequence))
                .Include(x => x.Product)
                .Include(x => x.ProductUoM)
                .ToListAsync();

            res.Lines = _mapper.Map<IEnumerable<ToaThuocLineDisplay>>(lines);

            return res;
        }

        public async Task<ToaThuocDisplay> DefaultGet(ToaThuocDefaultGet val)
        {
            var res = new ToaThuocDisplay();
            res.CompanyId = CompanyId;
            var employeeObj = GetService<IEmployeeService>();
            var userId = UserId;

            var employee = await employeeObj.SearchQuery(x => x.IsDoctor && x.UserId == userId).FirstOrDefaultAsync();
            if (employee != null)
            {
                res.EmployeeId = employee.Id;
                res.Employee = _mapper.Map<EmployeeBasic>(employee);
            }

            if (val.DotKhamId.HasValue)
            {
                var dkObj = GetService<IDotKhamService>();
                var dk = await dkObj.SearchQuery(x => x.Id == val.DotKhamId)
                    .FirstOrDefaultAsync();
                if (dk.PartnerId.HasValue)
                    res.PartnerId = dk.PartnerId.Value;
                res.DotKhamId = dk.Id;
                res.DotKham = _mapper.Map<DotKhamSimple>(dk);
            }

            if (val.PartnerId.HasValue)
            {
                var partnerObj = GetService<IPartnerService>();
                var partner = await partnerObj.GetByIdAsync(val.PartnerId);
                res.PartnerId = partner.Id;
                res.Partner = _mapper.Map<PartnerCustomerDonThuoc>(partner);
            }

            if (val.SaleOrderId.HasValue)
            {
                res.SaleOrderId = val.SaleOrderId;
            }

            return res;
        }

        public async Task<ToaThuocLineDisplay> LineDefaultGet(ToaThuocLineDefaultGet val)
        {
            var res = new ToaThuocLineDisplay();
            if (val.ProductId.HasValue)
            {
                var productObj = GetService<IProductService>();
                var product = await productObj.SearchQuery(x => x.Id == val.ProductId).Include(x=>x.ProductUoMRels).Include(x => x.UOM)
                    .FirstOrDefaultAsync();
                res.ProductId = product.Id;
                res.Product = _mapper.Map<ProductBasic>(product);
                res.Note = product.KeToaNote;
            }
            return res;
        }

        public async override Task<ToaThuoc> CreateAsync(ToaThuoc entity)
        {

            var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
            entity.Name = await sequenceService.NextByCode("toa.thuoc");
            if (string.IsNullOrEmpty(entity.Name) || entity.Name == "/")
            {
                await InsertToaThuocSequence();
                entity.Name = await sequenceService.NextByCode("toa.thuoc");
            }

            var lineObj = GetService<IToaThuocLineService>();
            lineObj.ComputeName(entity.Lines);

            return await base.CreateAsync(entity);
        }

        public async Task<ToaThuocDisplay> GetToaThuocFromUIAsync(Guid id)
        {
            var toathuoc = await SearchQuery(x => x.Id == id).Include(x => x.SaleOrder).Include(x => x.Employee).Include(x => x.Partner).FirstOrDefaultAsync();
            if (toathuoc == null)
                return null;
            var toathuocDisplay = _mapper.Map<ToaThuocDisplay>(toathuoc);

            var toaThuocLineObj = GetService<IToaThuocLineService>();
            toathuocDisplay.Lines = await _mapper.ProjectTo<ToaThuocLineDisplay>(toaThuocLineObj.SearchQuery(x => x.ToaThuocId == id, orderBy: x => x.OrderBy(s => s.Sequence))).ToListAsync();

            return toathuocDisplay;
        }

        public async Task<ToaThuocBasic> CreateToaThuocFromUIAsync(ToaThuocSaveFromUI val)
        {
            var toathuoc = _mapper.Map<ToaThuoc>(val);
            SaveOrderLines(val, toathuoc);
            await CreateAsync(toathuoc);

            if (val.SaveSamplePrescription)
                await _SavePrescriptionSample(toathuoc, val.NameSamplePrescription);

            var result = _mapper.Map<ToaThuocBasic>(toathuoc);

            return result;
        }

        public async Task UpdateToaThuocFromUIAsync(Guid id, ToaThuocSaveFromUI val)
        {
            var toathuoc = await SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();
            if (toathuoc == null)
                return;

            //toathuoc = _mapper.Map(val, toathuoc); //không nên map như thế này, nên map thủ công
            toathuoc.ReExaminationDate = val.ReExaminationDate;
            toathuoc.EmployeeId = val.EmployeeId;
            toathuoc.Note = val.Note;

            SaveOrderLines(val, toathuoc);

            await Write(toathuoc);

            if (val.SaveSamplePrescription)
                await _SavePrescriptionSample(toathuoc, val.NameSamplePrescription);
        }

        private async Task _SavePrescriptionSample(ToaThuoc self, string name)
        {
            var samplePrescription = new SamplePrescription()
            {
                Name = name,
                Note = self.Note
            };

            foreach (var line in self.Lines)
            {
                samplePrescription.Lines.Add(new SamplePrescriptionLine
                {
                    ProductId = line.ProductId,
                    NumberOfTimes = line.NumberOfTimes,
                    AmountOfTimes = line.AmountOfTimes,
                    NumberOfDays = line.NumberOfDays,
                    Quantity = line.Quantity,
                    UseAt = line.UseAt,
                    ProductUoMId = line.ProductUoMId
                });
            }

            await _samplePrescriptionService.CreateAsync(samplePrescription);
        }

        public async Task Write(ToaThuoc entity)
        {
            var lineObj = GetService<IToaThuocLineService>();
            lineObj.ComputeName(entity.Lines);

            await UpdateAsync(entity);
        }

        public async Task<IEnumerable<ToaThuocBasic>> GetToaThuocsForDotKham(Guid dotKhamId)
        {
            var toaThuocs = await _mapper.ProjectTo<ToaThuocBasic>(SearchQuery(x => x.DotKhamId == dotKhamId)).ToListAsync();
            return toaThuocs;
        }

        public async Task<ToaThuoc> GetToaThuocPrint(Guid id)
        {
            var toaThuoc = await SearchQuery(x => x.Id == id)
                .Include(x => x.Company.Partner)
                .Include(x => x.Employee)
                .Include(x => x.Lines).ThenInclude(x => x.Product.UOM)
                .Include(x => x.Partner).FirstOrDefaultAsync();

            return toaThuoc;
        }

        private async Task InsertToaThuocSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "toa.thuoc",
                Name = "Mã toa thuốc",
                Prefix = "TT",
                Padding = 6,
            });
        }

        public override ISpecification<ToaThuoc> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "sale.toa_thuoc_comp_rule":
                    return new InitialSpecification<ToaThuoc>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }

        public async Task<PagedResult2<ToaThuocBasic>> GetPagedResultAsync(ToaThuocPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) ||
                   x.Partner.Name.Contains(val.Search) || x.Partner.DisplayName.Contains(val.Search) | x.Partner.Ref.Contains(val.Search));

            if (val.PartnerId.HasValue)
                query = query.Where(x => x.PartnerId == val.PartnerId);
            if (val.SaleOrderId.HasValue)
            {
                query = query.Where(x => x.SaleOrder.Id == val.SaleOrderId);
            }

            if (val.DateFrom.HasValue)
                query = query.Where(x => x.Date >= val.DateFrom);

            if (val.DateTo.HasValue)
            {
                var dateOrderTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.Date <= dateOrderTo);
            }

            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await _mapper.ProjectTo<ToaThuocBasic>(query.OrderByDescending(x => x.DateCreated)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ToaThuocBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        private void SaveOrderLines(ToaThuocSaveFromUI val, ToaThuoc order)
        {
            var lineToRemoves = new List<ToaThuocLine>();

            foreach (var existLine in order.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                order.Lines.Remove(line);
            }

            int sequence = 1;

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var l = _mapper.Map<ToaThuocLine>(line);
                    l.Sequence = sequence;
                    order.Lines.Add(l);
                }
                else
                {
                    var l = order.Lines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                    l.Sequence = sequence;
                }

                sequence++;
            }
        }
    }
}
