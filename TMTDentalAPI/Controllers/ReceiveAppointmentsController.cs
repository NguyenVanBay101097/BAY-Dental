using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReceiveAppointmentsController : BaseApiController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ICustomerReceiptService _customerReceiptService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMapper _mapper;

        public ReceiveAppointmentsController(IAppointmentService appointmentService,
            IMapper mapper,
            ICustomerReceiptService customerReceiptService,
            IUnitOfWorkAsync unitOfWork)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _customerReceiptService = customerReceiptService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet([FromQuery]Guid appointmentId)
        {
            var appointment = await _appointmentService.SearchQuery(x => x.Id == appointmentId)
                .Include(x => x.Partner)
                .Include(x => x.Doctor)
                .Include(x => x.AppointmentServices).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();

            var res = new ReceiveAppointmentDisplayViewModel
            {
                Doctor = _mapper.Map<EmployeeSimple>(appointment.Doctor),
                IsRepeatCustomer = appointment.IsRepeatCustomer,
                Note = appointment.Note,
                Partner = _mapper.Map<PartnerSimple>(appointment.Partner),
                Products = appointment.AppointmentServices.Select(x => _mapper.Map<ProductSimple>(x.Product)),
                TimeExpected = appointment.TimeExpected,
                AppointmentId = appointmentId,
            };

            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionSave(ReceiveAppointmentSaveViewModel val)
        {
            var appointment = await _appointmentService.GetByIdAsync(val.AppointmentId);

            var customerReceipt = new CustomerReceipt()
            {
                PartnerId = val.PartnerId,
                Note = val.Note,
                TimeExpected = val.TimeExpected,
                DateWaiting = val.DateWaiting,
                CompanyId = appointment.CompanyId,
                DoctorId = val.DoctorId,
                IsRepeatCustomer = val.IsRepeatCustomer,
                State = "waiting",
            };

            foreach (var productId in val.ProductIds)
                customerReceipt.CustomerReceiptProductRels.Add(new CustomerReceiptProductRel { ProductId = productId });

            await _unitOfWork.BeginTransactionAsync();

            await _customerReceiptService.CreateAsync(customerReceipt);

            if (appointment.State != "done")
            {
                appointment.State = "done";
                await _appointmentService.UpdateAsync(appointment);
            }

            _unitOfWork.Commit();

            return Ok(_mapper.Map<CustomerReceiptBasic>(customerReceipt));
        }
    }
}
