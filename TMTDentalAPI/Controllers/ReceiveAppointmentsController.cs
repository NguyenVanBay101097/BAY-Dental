using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.Hubs;
using TMTDentalAPI.JobFilters;
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
        private readonly IHubContext<AppointmentHub> _appointmentHubContext;
        private readonly AppTenant _tenant;

        public ReceiveAppointmentsController(IAppointmentService appointmentService,
            IMapper mapper,
            ICustomerReceiptService customerReceiptService,
            IUnitOfWorkAsync unitOfWork,
            IHubContext<AppointmentHub> appointmentHubContext,
            IOptions<AppTenant> tenant)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _customerReceiptService = customerReceiptService;
            _unitOfWork = unitOfWork;
            _appointmentHubContext = appointmentHubContext;
            _tenant = tenant?.Value;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet([FromQuery]Guid appointmentId)
        {
            var appointment = await _appointmentService.SearchQuery(x => x.Id == appointmentId)
                .Include(x => x.Partner)
                .Include(x => x.Doctor)
                .Include(x => x.AppointmentServices).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();

            if (appointment.State == "cancel" || appointment.Date < DateTime.Today)
                throw new Exception("Không thể tiếp nhận lịch hẹn ở trạng thái hủy hẹn hoặc quá hẹn");

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
        [CheckAccess(Actions = "Basic.CustomerReceipt.Create")]
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

            var appointmentUpdated = false;
            if (appointment.State != "done")
            {
                appointment.State = "done";
                await _appointmentService.UpdateAsync(appointment);
                appointmentUpdated = true;
            }

            _unitOfWork.Commit();

            if (appointmentUpdated)
            {
                appointment = await _appointmentService.SearchQuery(x => x.Id == appointment.Id)
                    .Include(x => x.Partner)
                    .Include(x => x.Doctor)
                    .FirstOrDefaultAsync();
                var basic = _mapper.Map<AppointmentBasic>(appointment);
                await _appointmentHubContext.Clients.Groups(GetGroupName(appointment)).SendAsync("updated", basic);
            }

            return Ok(_mapper.Map<CustomerReceiptBasic>(customerReceipt));
        }

        private string GetGroupName(Appointment appointment)
        {
            return $"{(_tenant?.Id.ToString() ?? "localhost")}-{appointment.CompanyId}";
        }
    }
}
