using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using TMTDentalAPI.Services;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : BaseApiController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMailMessageService _mailMessageService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INotificationHubService _notificationHubService;
        private readonly IDotKhamService _dotKhamService;

        public AppointmentsController(IAppointmentService appointmentService,
            IMapper mapper, UserManager<ApplicationUser> userManager,
            IMailMessageService mailMessageService,
            IUnitOfWorkAsync unitOfWork,
            INotificationHubService notificationHubService,
            IDotKhamService dotKhamService)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _userManager = userManager;
            _mailMessageService = mailMessageService;
            _unitOfWork = unitOfWork;
            _notificationHubService = notificationHubService;
            _dotKhamService = dotKhamService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.Appointment.Read")]
        public async Task<IActionResult> Get([FromQuery] AppointmentPaged appointmentPaged)
        {
            var result = await _appointmentService.GetPagedResultAsync(appointmentPaged);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.Appointment.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _appointmentService.GetAppointmentDisplayAsync(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.Appointment.Create")]
        public async Task<IActionResult> Create(AppointmentDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            var res = await _appointmentService.CreateAsync(val);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<AppointmentDisplay>(res));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.Appointment.Update")]
        public async Task<IActionResult> Update(Guid id, AppointmentDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _appointmentService.UpdateAsync(id, val);

            return NoContent();
        }

        ///Cập nhật trạng thái lịch hẹn đã quá hạn
        [HttpPatch("PatchMulti")]
        public async Task<IActionResult> PatchMulti(PagedResult2<AppointmentBasic> result)
        {
            var list = _mapper.Map<IEnumerable<AppointmentPatch>>(result);
            foreach (var item in list)
            {
                var entity = await _appointmentService.GetByIdAsync(item.Id);
                if (entity == null)
                {
                    return NotFound();
                }
                var patch = new JsonPatchDocument<AppointmentPatch>();
                patch.Replace(x => x.State, "expired");
                var entityMap = _mapper.Map<AppointmentPatch>(entity);
                patch.ApplyTo(entityMap);

                entity = _mapper.Map(entityMap, entity);
                await _appointmentService.UpdateAsync(entity);
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        [CheckAccess(Actions = "Basic.Appointment.Update")]
        public async Task<IActionResult> Patch(Guid id, JsonPatchDocument<AppointmentPatch> apnPatch)
        {
            var entity = await _appointmentService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var entityMap = _mapper.Map<AppointmentPatch>(entity);
            apnPatch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            await _appointmentService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.Appointment.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();

            await _appointmentService.DeleteAsync(appointment);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(AppointmentDefaultGet val)
        {
            var res = await _appointmentService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("CountAppointment")]
        public async Task<IActionResult> CountAppointment(DateFromTo dateFromTo)
        {
            var res = await _appointmentService.CountAppointment(dateFromTo);
            return Ok(res);
        }

        [HttpPost("Count")]
        public async Task<IActionResult> GetCount(AppointmentGetCountVM val)
        {
            var res = await _appointmentService.GetCount(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SearchRead(AppointmentSearch val)
        {
            var res = await _appointmentService.SearchRead(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Appointment.Read")]
        public async Task<IActionResult> SearchReadByDate(AppointmentSearchByDate val)
        {
            var res = await _appointmentService.SearchReadByDate(val);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetBasic(Guid id)
        {
            var res = await _appointmentService.GetBasic(id);
            if (res == null)
                return NotFound();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CheckForthcoming()
        {
            var minutes = 15;
            var now = DateTime.Now;
            var date = now.AddMinutes(minutes);
            var appointments = await _appointmentService.SearchQuery(x => x.Date >= now && x.Date <= date && !x.AppointmentMailMessageRels.Any()).Include(x => x.User).ToListAsync();
            var user = await _userManager.FindByIdAsync(UserId);
            var messages = new List<MailMessage>();
            foreach (var ap in appointments)
            {
                var message = new MailMessage()
                {
                    AuthorId = user.PartnerId,
                    Subject = $"Lịch hẹn sắp tới giờ hẹn",
                    Body = $"Lịch hẹn với mã <span class=\"message-bold\">{ap.Name}</span> còn <span class=\"message-bold\">{(ap.Date - now).Minutes} phút</span> nữa sẽ tới giờ hẹn",
                    MessageType = "notification",
                    Model = "appointment",
                    ResId = ap.Id,
                    RecordName = ap.Name,
                };

                message.Recipients.Add(new MailMessageResPartnerRel { PartnerId = ap.User.PartnerId });
                message.Notifications.Add(new MailNotification { ResPartnerId = ap.User.PartnerId });

                messages.Add(message);

                ap.AppointmentMailMessageRels.Add(new AppointmentMailMessageRel { MailMessage = message });
            }

            await _unitOfWork.BeginTransactionAsync();
            await _mailMessageService.CreateAsync(messages);

            await _appointmentService.UpdateAsync(appointments);

            var formatMessages = await _mailMessageService.MessageFormat(messages.Select(x => x.Id).ToList());
            foreach (var message in formatMessages)
            {
                await _notificationHubService.BroadcastNotificationAsync(message);
            }
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var res = await _appointmentService.GetBasic(id);
            return Ok(res);
        }

        [HttpPatch("{id}/[action]")]
        public async Task<IActionResult> PatchState(Guid id, AppointmentStatePatch result)
        {
            var entity = await _appointmentService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var patch = new JsonPatchDocument<AppointmentStatePatch>();
            patch.Replace(x => x.State, result.State);
            patch.Replace(x => x.Reason, result.Reason);
            var entityMap = _mapper.Map<AppointmentStatePatch>(entity);
            patch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            await _appointmentService.UpdateAsync(entity);

            return NoContent();
        }

        [CheckAccess(Actions = "Basic.Appointment.Update")]
        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcel([FromQuery] AppointmentPaged appointmentPaged)
        {
            var data = await _appointmentService.GetExcelData(appointmentPaged);
            var stream = new MemoryStream();
            string sheetName = "";
            byte[] fileContent;
            var stateDict = new Dictionary<string, string>() {
                {"confirmed", "Đang hẹn" },
                {"waiting", "Chờ khám" },
                {"examination", "Đang khám" },
                {"done", "Hoàn thành" },
                {"cancel", "Hủy hẹn" },
            };

            using (var package = new ExcelPackage(stream))
            {
                if (!appointmentPaged.Cancel)
                {
                    foreach (var group in data.GroupBy(x => x.Date).OrderBy(x => x.Key))
                    {
                        sheetName = group.Key.Value.ToString("dddd, dd-MM-yyyy", new CultureInfo("vi-VN"));
                        var worksheet = package.Workbook.Worksheets.Add(sheetName);
                        _appointmentService.ComputeDataExcel(worksheet, group.ToList(), stateDict);
                    }
                }
                else
                {
                    sheetName = appointmentPaged.State == "cancel" ? "Hủy hẹn" : (appointmentPaged.State == "confirmed" ? "Quá hạn" : "Quá hạn - hủy hẹn");
                    var worksheet = package.Workbook.Worksheets.Add(sheetName);
                    _appointmentService.ComputeDataExcel(worksheet, data, stateDict);
                }

                package.Save();
                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }



    }
}