using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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
    public class SalaryPaymentService : BaseService<SalaryPayment>, ISalaryPaymentService
    {
        private readonly IMapper _mapper;

        public SalaryPaymentService(IAsyncRepository<SalaryPayment> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }



        public async Task<SalaryPayment> CreateSalaryPayment(SalaryPaymentSave val)
        {
            var salaryPayment = _mapper.Map<SalaryPayment>(val);

            if (string.IsNullOrEmpty(salaryPayment.Name))
            {
                var seqObj = GetService<IIRSequenceService>();
                if (salaryPayment.Type == "advance")
                {
                    salaryPayment.Name = await seqObj.NextByCode("salarypayment.advance");
                    if (string.IsNullOrEmpty(salaryPayment.Name))
                    {
                        await _InsertSalaryPaymentAdvanceSequence();
                        salaryPayment.Name = await seqObj.NextByCode("salarypayment.advance");
                    }
                }
                else if (salaryPayment.Type == "salary")
                {
                    salaryPayment.Name = await seqObj.NextByCode("salarypayment.salary");
                    if (string.IsNullOrEmpty(salaryPayment.Name))
                    {
                        await _InsertSalaryPaymentSalarySequence();
                        salaryPayment.Name = await seqObj.NextByCode("salarypayment.salary");
                    }
                }
                else
                    throw new Exception("Not support");
            }

            salaryPayment.CompanyId = CompanyId;

            return await CreateAsync(salaryPayment);
        }





        private async Task _InsertSalaryPaymentAdvanceSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu tạm ứng",
                Code = "salarypayment.advance",
                Prefix = "ADVANCE/{yyyy}/",
                Padding = 4
            });
        }

        private async Task _InsertSalaryPaymentSalarySequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Name = "Phiếu chi lương",
                Code = "salarypayment.salary",
                Prefix = "PAYWAGE/{yyyy}/",
                Padding = 4
            });
        }

        public async Task UpdateSalaryPayment(Guid id, SalaryPaymentSave val)
        {
            var salaryPayment = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (salaryPayment == null)
                throw new Exception("Phiếu không tồn tại");

            salaryPayment = _mapper.Map(val, salaryPayment);

            await UpdateAsync(salaryPayment);
        }

        public override async Task<SalaryPayment> CreateAsync(SalaryPayment entity)
        {
            var employeeObj = GetService<IEmployeeService>();
            var partnerObj = GetService<IPartnerService>();
            var model = await base.CreateAsync(entity);

            var employee = await employeeObj.SearchQuery(x => x.Id == entity.Id).FirstOrDefaultAsync();
            if (!employee.PartnerId.HasValue)
            {
                var partner = new Partner()
                {
                    Name = employee.Name,
                    Customer = false,
                    Supplier = false,
                };

                await partnerObj.CreateAsync(partner);
                employee.PartnerId = partner.Id;
                await employeeObj.UpdateAsync(employee);
            }

            return model;
        }

        public override async Task UpdateAsync(SalaryPayment entity)
        {
            var employeeObj = GetService<IEmployeeService>();
            var partnerObj = GetService<IPartnerService>();
            await base.UpdateAsync(entity);

            var employee = await employeeObj.SearchQuery(x => x.Id == entity.Id).FirstOrDefaultAsync();
            if (!employee.PartnerId.HasValue)
            {
                var partner = new Partner()
                {
                    Name = employee.Name,
                    Customer = false,
                    Supplier = false,
                };

                await partnerObj.CreateAsync(partner);
                employee.PartnerId = partner.Id;
                await employeeObj.UpdateAsync(employee);
            }
        }
    }
}
