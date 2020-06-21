using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class SamplePrescriptionService : BaseService<SamplePrescription>, ISamplePrescriptionService
    {
        private readonly IMapper _mapper;
        public SamplePrescriptionService(IAsyncRepository<SamplePrescription> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<SamplePrescriptionBasic>> GetPagedResultAsync(SamplePrescriptionPaged val)
        {
            ISpecification<SamplePrescription> spec = new InitialSpecification<SamplePrescription>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<SamplePrescription>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<SamplePrescriptionBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<SamplePrescriptionBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SamplePrescription> GetPrescription(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Lines)
                .Include("Lines.Product")
                .FirstOrDefaultAsync();
        }

        public async Task<SamplePrescription> CreatePrescription(SamplePrescriptionSave val)
        {
            var prescription = _mapper.Map<SamplePrescription>(val);

            SaveLines(val, prescription);

            return await CreateAsync(prescription);
        }

        public async Task UpdatePrescription(Guid id, SamplePrescriptionSave val)
        {
            var prescription = await SearchQuery(x => x.Id == id).Include(x => x.Lines).Include("Lines.Product").FirstOrDefaultAsync();
            if (prescription == null)
                throw new Exception("Đơn thuốc mẫu không tồn tại");

            prescription = _mapper.Map(val, prescription);
            SaveLines(val, prescription);

            await UpdateAsync(prescription);
        }

        private void SaveLines(SamplePrescriptionSave val, SamplePrescription prescription)
        {
            //remove line
            var lineToRemoves = new List<SamplePrescriptionLine>();
            foreach (var existLine in prescription.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                prescription.Lines.Remove(line);
            }

            int sequence = 1;
            foreach (var line in val.Lines)
                line.Sequence = sequence++;

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    prescription.Lines.Add(_mapper.Map<SamplePrescriptionLine>(line));
                }
                else
                {
                    _mapper.Map(line, prescription.Lines.SingleOrDefault(c => c.Id == line.Id));
                }
            }

        }

        public async Task<IEnumerable<SamplePrescriptionBasic>> GetAutocomplete(SamplePrescriptionPaged val)
        {
            ISpecification<SamplePrescription> spec = new InitialSpecification<SamplePrescription>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<SamplePrescription>(x => x.Name.Contains(val.Search)));
         

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<SamplePrescriptionBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            return items;
        }

    }
}
