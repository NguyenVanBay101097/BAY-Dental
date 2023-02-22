using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using TMTDentalAPI.Hubs;
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
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;
        private readonly IHubContext<AppointmentHub> _appointmentHubContext;
        private readonly AppTenant _tenant;

        public AppointmentsController(IAppointmentService appointmentService,
            IMapper mapper, UserManager<ApplicationUser> userManager,
            IMailMessageService mailMessageService,
            IUnitOfWorkAsync unitOfWork,
            INotificationHubService notificationHubService,
            IDotKhamService dotKhamService,
            IPrintTemplateConfigService printTemplateConfigService,
            IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService,
            IHubContext<AppointmentHub> appointmentHubContext,
            IOptions<AppTenant> tenant
            )
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
            _userManager = userManager;
            _mailMessageService = mailMessageService;
            _unitOfWork = unitOfWork;
            _notificationHubService = notificationHubService;
            _dotKhamService = dotKhamService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
            _appointmentHubContext = appointmentHubContext;
            _tenant = tenant?.Value;
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

            var basic = _mapper.Map<AppointmentBasic>(res);
            await _appointmentHubContext.Clients.Groups(GetGroupName(res)).SendAsync("created", basic);

            return Ok(basic);
        }

        private string GetGroupName(Appointment appointment)
        {
            return $"{(_tenant?.Id.ToString() ?? "localhost")}-{appointment.CompanyId}";
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.Appointment.Update")]
        public async Task<IActionResult> Update(Guid id, AppointmentDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var appointment = await _appointmentService.UpdateAsync(id, val);

            var apmt = await _appointmentService.GetByIdAsync(id);

            await _appointmentHubContext.Clients.Groups(GetGroupName(apmt)).SendAsync("updated", appointment);

            return Ok(appointment);
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

            await _appointmentHubContext.Clients.Groups(GetGroupName(appointment)).SendAsync("deleted", id);

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
                if (!appointmentPaged.IsLate.HasValue)
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

        [CheckAccess(Actions = "Basic.Appointment.Update")]
        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcel2([FromQuery] AppointmentPaged appointmentPaged)
        {
            var data = await _appointmentService.GetExcelData(appointmentPaged);
            var stream = new MemoryStream();
            byte[] fileContent;

            var stateDict = new Dictionary<string, string>() {
                {"confirmed", "Đang hẹn" },
                {"arrived", "Đã đến" },
                {"cancel", "Hủy hẹn" },
            };

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "Khách hàng";
                worksheet.Cells[1, 2].Value = "Số điện thoại";
                worksheet.Cells[1, 3].Value = "Thời gian hẹn";
                worksheet.Cells[1, 4].Value = "Dịch vụ";
                worksheet.Cells[1, 5].Value = "Bác sĩ";
                worksheet.Cells[1, 6].Value = "Nội dung";
                worksheet.Cells[1, 7].Value = "Loại khám";
                worksheet.Cells[1, 8].Value = "Trạng thái";
                worksheet.Cells[1, 9].Value = "Lý do";

                worksheet.Cells["A1:J1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data)
                {
                    var services = item.Services.ToArray();

                    List<string> servicesNameList = new List<string>();

                    foreach (var service in services)
                    {
                        servicesNameList.Add(service.Name);
                    }

                    worksheet.Cells[row, 1].Value = item.PartnerDisplayName;
                    worksheet.Cells[row, 2].Value = item.PartnerPhone;
                    worksheet.Cells[row, 3].Value = String.Format("{0:d/M/yyyy HH:mm}", item.Date);
                    worksheet.Cells[row, 4].Value = string.Join(", ", servicesNameList);
                    worksheet.Cells[row, 5].Value = item.DoctorName;
                    worksheet.Cells[row, 6].Value = item.Note;
                    worksheet.Cells[row, 7].Value = item.IsRepeatCustomer == true ? "Tái khám" : "Khám mới";
                    worksheet.Cells[row, 8].Value = !string.IsNullOrEmpty(item.State) && stateDict.ContainsKey(item.State) ? stateDict[item.State] : ""; ;
                    worksheet.Cells[row, 9].Value = item.Reason;

                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();
                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Basic.Appointment.Read")]
        public async Task<IActionResult> GetListDoctor([FromQuery] AppointmentDoctorReq val)
        {
            var result = await _appointmentService.GetListDoctor(val);
            return Ok(result);
        }

        [HttpGet("{id}/Print")]
        [CheckAccess(Actions = "Basic.Appointment.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_appointment" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_appointment");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }
            
            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);
            return Ok(new PrintData() { html = result });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\appointment.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample") && (x.Model == "employee" || x.Model == "partner" || x.Model == "sale.order" || x.Model == "product")).ToListAsync();// các irmodel cần thiết
            var entities = await _appointmentService.SearchQuery(x => x.Date.Date <= dateToData.Date).Include(x => x.AppointmentServices).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<AppointmentXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<AppointmentXmlSampleDataRecord>(entity);

                var partnerModelData = listIrModelData.FirstOrDefault(x => x.ResId == entity.PartnerId.ToString());
                var doctorModelData = listIrModelData.FirstOrDefault(x => x.ResId == entity.DoctorId.ToString());
                var orderModelData = listIrModelData.FirstOrDefault(x => x.ResId == entity.SaleOrderId.ToString());

                item.Id = $@"sample.appointment_{entities.IndexOf(entity) + 1}";
                item.DateRound = (int)(dateToData.Date - entity.Date.Date).TotalDays;
                item.TimeHour = entity.Date.Hour;
                item.TimeMinute = entity.Date.Minute;
                if (item.DateRound == 0)
                    item.State = "confirmed";
                item.PartnerId = partnerModelData == null ? "" : partnerModelData?.Module + "." + partnerModelData?.Name;
                item.DoctorId = doctorModelData == null ? "" : doctorModelData?.Module + "." + doctorModelData?.Name;
                item.SaleOrderId = orderModelData == null ? "" : orderModelData?.Module + "." + orderModelData?.Name;

                //add lines
                foreach (var lineEntity in entity.AppointmentServices)
                {
                    var irmodelDataProduct = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductId.ToString());
                    var itemLine = new ProductAppointmentRelXmlSampleDataRecord()
                    {
                        ProductId = irmodelDataProduct == null ? "" : irmodelDataProduct?.Module + "." + irmodelDataProduct?.Name
                    };
                    item.AppointmentServices.Add(itemLine);
                }

                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "appointment",
                    ResId = entity.Id.ToString(),
                    Name = $"appointment_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}