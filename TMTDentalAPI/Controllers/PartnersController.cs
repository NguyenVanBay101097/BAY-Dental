using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using Infrastructure;
using TMTDentalAPI.JobFilters;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : BaseApiController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IApplicationRoleFunctionService _roleFunctionService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IAccountInvoiceService _accountInvoiceService;
        private readonly IAccountCommonPartnerReportService _accReportService;
        private readonly IAccountPaymentService _paymentService;
        private readonly IServiceCardCardService _serviceCardService;
        private readonly IPartnerSourceService _partnerSourceService;
        private readonly IIRModelDataService _iRModelDataService;

        public PartnersController(IPartnerService partnerService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork,
            IPartnerCategoryService partnerCategoryService,
            IApplicationRoleFunctionService roleFunctionService,
            IAuthorizationService authorizationService,
            IIRModelAccessService modelAccessService,
            IAccountInvoiceService accountInvoiceService,
            IAccountPaymentService paymentService,
            IServiceCardCardService serviceCardService,
            IPartnerSourceService partnerSourceService,
            IIRModelDataService iRModelDataService,
            IAccountCommonPartnerReportService accReportService)
        {
            _accReportService = accReportService;
            _partnerService = partnerService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _partnerCategoryService = partnerCategoryService;
            _roleFunctionService = roleFunctionService;
            _authorizationService = authorizationService;
            _modelAccessService = modelAccessService;
            _accountInvoiceService = accountInvoiceService;
            _paymentService = paymentService;
            _serviceCardService = serviceCardService;
            _partnerSourceService = partnerSourceService;
            _iRModelDataService = iRModelDataService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> Get([FromQuery] PartnerPaged val)
        {
            var result = await _partnerService.GetPagedResultAsync(val);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var partner = await _partnerService.GetPartnerForDisplayAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<PartnerDisplay>(partner);
            res.City = new CitySimple { Code = partner.CityCode, Name = partner.CityName };
            res.District = new DistrictSimple { Code = partner.DistrictCode, Name = partner.DistrictName };
            res.Ward = new WardSimple { Code = partner.WardCode, Name = partner.WardName };
            return Ok(res);
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(PartnerDefaultGet val)
        {
            var res = new PartnerDisplay();
            var maleTitle = _iRModelDataService.GetRef<PartnerTitle>("base.partner_title_man").Result;
            if (maleTitle != null)
            {
                res.Title = _mapper.Map<PartnerTitleBasic>(maleTitle);
                res.TitleId = maleTitle.Id;
            }

            res.CompanyId = CompanyId;
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDefaultTitle(string gender)
        {
            PartnerTitle title = null;
            if (gender == "male")
                title = await _iRModelDataService.GetRef<PartnerTitle>("base.partner_title_man");
            else if (gender == "female")
                title = await _iRModelDataService.GetRef<PartnerTitle>("base.partner_title_woman");

            var basic = title != null ? _mapper.Map<PartnerTitleBasic>(title) : null;
            return Ok(basic);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.Partner.Create")]
        public async Task<IActionResult> Create(PartnerDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var partner = _mapper.Map<Partner>(val);
            CityDistrictWardPrepare(partner, val);

            partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(partner.Name);

            SaveCategories(val, partner);
            SaveHistories(val, partner);
            await _partnerService.CreateAsync(partner);

            _unitOfWork.Commit();

            var basic = _mapper.Map<PartnerBasic>(partner);
            return Ok(basic);
        }

        private void FixCityName(Partner partner)
        {
            if (!string.IsNullOrEmpty(partner.CityName) && partner.CityName.Contains("TP"))
                partner.CityName = partner.CityName.Replace("TP", "Thành phố");
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public async Task<IActionResult> Update(Guid id, PartnerDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var partner = await _partnerService.GetPartnerForDisplayAsync(id);
            if (partner == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            partner = _mapper.Map(val, partner);

            CityDistrictWardPrepare(partner, val);

            partner.NameNoSign = StringUtils.RemoveSignVietnameseV2(partner.Name);

            SaveCategories(val, partner);
            SaveHistories(val, partner);
            await _partnerService.UpdateAsync(partner);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.Partner.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var partner = await _partnerService.GetByIdAsync(id);
            if (partner == null)
                return NotFound();
            await _partnerService.DeleteAsync(partner);

            return NoContent();
        }

        [HttpGet("Autocomplete")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> Autocomplete(string filter = "", bool? customer = null)
        {
            var res = await _partnerService.SearchAutocomplete(filter: filter, customer: customer);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult FetchAllPSIDFromFacebookFanpage()
        {
            _partnerService.FetchAllPSIDFromFacebookFanpage();
            return Ok(true);
        }

        [HttpPost("Autocomplete2")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> Autocomplete2(PartnerPaged val)
        {
            var res = await _partnerService.SearchPartnersCbx(val);
            res = res.Skip(val.Offset).Take(val.Limit);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> Autocomplete3(PartnerPaged val)
        {
            var res = await _partnerService.SearchPartnersConnectSocial(val);
            res = res.Skip(val.Offset).Take(val.Limit);
            return Ok(res);
        }

        [HttpPost("AutocompleteSimple")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> AutocompleteSimple(PartnerPaged val)
        {
            var res = await _partnerService.SearchPartnersCbx(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> AutocompleteInfos(PartnerPaged val)
        {
            var res = await _partnerService.SearchPartnerInfosCbx(val);
            return Ok(res);
        }

        [HttpPost("UploadImage/{id}")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
        {
            var path = await _partnerService.UploadImage(file);

            var entity = await _partnerService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var patch = new JsonPatchDocument<PartnerPatch>();
            patch.Replace(x => x.Avatar, path);
            var entityMap = _mapper.Map<PartnerPatch>(entity);
            patch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);

            await _partnerService.UpdateAsync(entity);

            return Ok();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public async Task<IActionResult> AddTags(PartnerAddRemoveTagsVM val)
        {
            await _partnerService.AddOrRemoveTags(val, true);
            return Ok();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public async Task<IActionResult> RemoveTags(PartnerAddRemoveTagsVM val)
        {
            await _partnerService.AddOrRemoveTags(val, false);
            return Ok();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public async Task<IActionResult> UpdateTags(PartnerAddRemoveTagsVM val)
        {
            await _partnerService.UpdateTags(val);
            return Ok();
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> GetValidServiceCards(Guid id)
        {
            var cards = await _mapper.ProjectTo<ServiceCardCardBasic>(_serviceCardService.SearchQuery(x => x.PartnerId == id && x.Residual > 0)).ToListAsync();
            return Ok(cards);
        }

        private void SaveCategories(PartnerDisplay val, Partner partner)
        {
            var toRemove = partner.PartnerPartnerCategoryRels.Where(x => !val.Categories.Any(s => s.Id == x.CategoryId)).ToList();
            foreach (var categ in toRemove)
            {
                partner.PartnerPartnerCategoryRels.Remove(categ);
            }
            if (val.Categories != null)
            {
                foreach (var categ in val.Categories)
                {
                    if (partner.PartnerPartnerCategoryRels.Any(x => x.CategoryId == categ.Id))
                        continue;
                    partner.PartnerPartnerCategoryRels.Add(_mapper.Map<PartnerPartnerCategoryRel>(categ));

                }
            }

        }

        private void SaveHistories(PartnerDisplay val, Partner partner)
        {
            var toRemove = partner.PartnerHistoryRels.Where(x => !val.Histories.Any(s => s.Id == x.HistoryId)).ToList();
            foreach (var hist in toRemove)
            {
                partner.PartnerHistoryRels.Remove(hist);
            }
            if (val.Histories != null)
            {
                foreach (var hist in val.Histories)
                {
                    if (partner.PartnerHistoryRels.Any(x => x.HistoryId == hist.Id))
                        continue;
                    partner.PartnerHistoryRels.Add(_mapper.Map<PartnerHistoryRel>(hist));

                }
            }

        }


        [HttpGet("{id}/GetInfo")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> GetInfo(Guid id)
        {
            var res = await _partnerService.GetInfo(id);
            return Ok(res);
        }

        //Lấy tất cả hóa đơn của KH 
        [HttpPost("GetCustomerInvoices")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> GetCustomerInvoice(AccountInvoicePaged val)
        {
            var res = await _partnerService.GetCustomerInvoices(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Create")]
        public async Task<IActionResult> ExcelImportCreate(IFormFile file, [FromQuery] Ex_ImportExcelDirect dir)
        {
            await _partnerService.ImportExcel2(file, dir);
            return Ok();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Create")]
        public async Task<IActionResult> ActionImport(PartnerImportExcelViewModel val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var result = await _partnerService.ActionImport(val);

            if (result.Success)
                _unitOfWork.Commit();

            return Ok(result);
        }

        [HttpGet("{id}/Print")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _partnerService.GetPrint(id);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExcelImportUpdate(IFormFile file, [FromQuery] Ex_ImportExcelDirect dir)
        {
            await _partnerService.ImportExcel2(file, dir);
            return Ok();
        }

        //Check địa chỉ 
        [HttpGet("CheckAddress")]
        public async Task<IActionResult> CheckAddress([FromQuery] string text)
        {
            //HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                response = await client.GetAsync("http://dc.tpos.vn/home/checkaddress?address=" + text);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsAsync<IEnumerable<AddressCheckApi>>();
                    return Ok(res);
                }
                else
                {
                    return Ok(new List<AddressCheckApi>());
                }
            }

        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetDefaultRegisterPayment(Guid id)
        {
            var rec = await _paymentService.PartnerDefaultGet(id);
            return Ok(rec);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDefaultPayment(PartnerDefaultSearch val)
        {
            var rec = await _paymentService.PartnerDefaultGetV2(val);
            return Ok(rec);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelFileNCC(AccountCommonPartnerReportSearch val)
        {
            var stream = new MemoryStream();
            var data = await _accReportService.GetListReportPartner(val);
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Nguồn";
                worksheet.Cells[1, 3].Value = "Tổng tiền";
                worksheet.Cells[1, 4].Value = "Thanh toán";
                worksheet.Cells[1, 5].Value = "Còn nợ";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.InvoiceDate;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.InvoiceOrigin;
                    worksheet.Cells[row, 3].Value = item.AmountTotal;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##";
                    worksheet.Cells[row, 4].Value = item.AmountTotal - item.AmountResidual;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,##";
                    worksheet.Cells[row, 5].Value = item.AmountResidual;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##";

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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerLocation")]
        public async Task<IActionResult> ReportLocationCompanyWard(PartnerReportLocationCompanySearch val)
        {
            var res = await _partnerService.ReportLocationCompanyWard(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Report.PartnerLocation")]
        public async Task<IActionResult> ReportLocationCompanyDistrict(PartnerReportLocationCompanySearch val)
        {
            var res = await _partnerService.ReportLocationCompanyDistrict(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ReportLocationCity(ReportLocationCitySearch val)
        {
            var res = await _partnerService.ReportLocationCity(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ReportLocationDistrict(PartnerReportLocationCity val)
        {
            var res = await _partnerService.ReportLocationDistrict(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ReportLocationWard(PartnerReportLocationDistrict val)
        {
            var res = await _partnerService.ReportLocationWard(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public async Task<IActionResult> UpdateCustomersZaloId()
        {
            await _unitOfWork.BeginTransactionAsync();
            await _partnerService.UpdateCustomersZaloId();
            _unitOfWork.Commit();
            return NoContent();
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IEnumerable<PartnerDisplay>> GetPartnerDisplaysByIds(IEnumerable<Guid> ids)
        {
            var entity = await _partnerService.SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var res = _mapper.Map<IEnumerable<PartnerDisplay>>(entity);

            return res;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> ExportExcelFile(PartnerPaged val)
        {
            var stream = new MemoryStream();
            var data = await _partnerService.GetExcel(val);
            byte[] fileContent;

            var gender_dict = new Dictionary<string, string>()
            {
                { "male", "Nam" },
                { "female", "Nữ" },
                { "other", "Khác" }
            };

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Tên KH";
                worksheet.Cells[1, 2].Value = "Mã KH";
                worksheet.Cells[1, 3].Value = "Ngày tạo";
                worksheet.Cells[1, 4].Value = "Giới tính";
                worksheet.Cells[1, 5].Value = "Ngày sinh";
                worksheet.Cells[1, 6].Value = "Tháng sinh";
                worksheet.Cells[1, 7].Value = "Năm sinh";
                worksheet.Cells[1, 8].Value = "SĐT";
                worksheet.Cells[1, 9].Value = "Đường";
                worksheet.Cells[1, 10].Value = "Phường/Xã";
                worksheet.Cells[1, 11].Value = "Quận/Huyện";
                worksheet.Cells[1, 12].Value = "Tỉnh/Thành";
                worksheet.Cells[1, 13].Value = "Tiểu sử bệnh";
                worksheet.Cells[1, 14].Value = "Nghề nghiệp";
                worksheet.Cells[1, 15].Value = "Email";
                worksheet.Cells[1, 16].Value = "Ghi chú";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = item.Ref;
                    worksheet.Cells[row, 3].Value = item.Date;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 4].Value = !string.IsNullOrEmpty(item.Gender) && gender_dict.ContainsKey(item.Gender) ? gender_dict[item.Gender] : "Nam";
                    worksheet.Cells[row, 5].Value = item.BirthDay;
                    worksheet.Cells[row, 6].Value = item.BirthMonth;
                    worksheet.Cells[row, 7].Value = item.BirthYear;
                    worksheet.Cells[row, 8].Value = item.Phone;
                    worksheet.Cells[row, 9].Value = item.Street;
                    worksheet.Cells[row, 10].Value = item.WardName;
                    worksheet.Cells[row, 11].Value = item.DistrictName;
                    worksheet.Cells[row, 12].Value = item.CityName;
                    worksheet.Cells[row, 13].Value = string.Join(",", item.MedicalHistories);
                    worksheet.Cells[row, 14].Value = item.Job;
                    worksheet.Cells[row, 15].Value = item.Email;
                    worksheet.Cells[row, 16].Value = item.Note;

                    row++;
                }

                worksheet.Column(8).Style.Numberformat.Format = "@";
                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        private void CityDistrictWardPrepare(Partner partner, PartnerDisplay val)
        {
            partner.CityCode = val.City != null ? val.City.Code : string.Empty;
            partner.CityName = val.City != null ? val.City.Name : string.Empty;
            partner.DistrictCode = val.District != null ? val.District.Code : string.Empty;
            partner.DistrictName = val.District != null ? val.District.Name : string.Empty;
            partner.WardCode = val.Ward != null ? val.Ward.Code : string.Empty;
            partner.WardName = val.Ward != null ? val.Ward.Name : string.Empty;
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.Appointment.Read")]
        public async Task<IActionResult> GetNextAppointment(Guid id)
        {
            var res = await _partnerService.GetNextAppointment(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public async Task<IActionResult> SaveAvatar(PartnerSaveAvatarVM val)
        {
            var partner = await _partnerService.GetByIdAsync(val.PartnerId);
            partner.Avatar = val.ImageId;
            await _partnerService.UpdateAsync(partner);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> PartnerCustomerReport(PartnerCustomerReportInput val)
        {
            var result = await _partnerService.GetPartnerCustomerReportV2(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> CustomerStatistics(CustomerStatisticsInput val)
        {
            var result = await _partnerService.GetCustomerStatistics(val);
            return Ok(result);
        }

        //lấy danh sách hóa đơn còn nợ của 1 partner
        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetUnreconcileInvoices(Guid id, [FromQuery]string search = "")
        {
            var moves = await _partnerService.GetUnreconcileInvoices(id, search);
            return Ok(_mapper.Map<IEnumerable<AccountMoveBasic>>(moves));
        }
    }
}