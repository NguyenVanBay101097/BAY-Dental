using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiThuChiController : BaseApiController
    {
        private readonly ILoaiThuChiService _loaiThuChiService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public LoaiThuChiController(ILoaiThuChiService loaiThuChiService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _loaiThuChiService = loaiThuChiService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //api get phan trang loai thu , chi
        [HttpGet]
        [CheckAccess(Actions = "Account.LoaiThuChi.Read")]
        public async Task<IActionResult> GetLoaiThuChi([FromQuery] LoaiThuChiPaged val)
        {
            var res = await _loaiThuChiService.GetThuChiPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Account.LoaiThuChi.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _loaiThuChiService.GetByIdThuChi(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(LoaiThuChiDefault val)
        {
            var res = _loaiThuChiService.DefaultGet(val);
            return Ok(res);
        }

        //api create
        [HttpPost]
        [CheckAccess(Actions = "Account.LoaiThuChi.Create")]
        public async Task<IActionResult> Create(LoaiThuChiSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var loaithuchi = await _loaiThuChiService.CreateLoaiThuChi(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<LoaiThuChiBasic>(loaithuchi);
            return Ok(basic);
        }

        //api update
        [HttpPut("{id}")]
        [CheckAccess(Actions = "Account.LoaiThuChi.Update")]
        public async Task<IActionResult> Update(Guid id, LoaiThuChiSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _loaiThuChiService.UpdateLoaiThuChi(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        //api xóa
        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Account.LoaiThuChi.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _loaiThuChiService.RemoveLoaiThuChi(id);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateLoaiThuXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\loai_thu.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 26);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample") && (x.Model == "account.journal" || x.Model == "account.account")).ToListAsync();// các irmodel cần thiết 
            var entities = await _loaiThuChiService.SearchQuery(x => x.Type == "thu" && x.DateCreated.Value.Date <= dateToData.Date).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<LoaiThuChiXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<LoaiThuChiXmlSampleDataRecord>(entity);
                item.Id = $@"sample.loai_thu_chi_thu_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "loai.thu.chi",
                    ResId = entity.Id.ToString(),
                    Name = $"loai_thu_chi_thu_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateLoaiChiXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\loai_chi.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 26);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample") && (x.Model == "account.journal" || x.Model == "account.account")).ToListAsync();// các irmodel cần thiết 
            var entities = await _loaiThuChiService.SearchQuery(x => x.Type == "chi" && x.DateCreated.Value.Date <= dateToData.Date).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<LoaiThuChiXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<LoaiThuChiXmlSampleDataRecord>(entity);
                item.Id = $@"sample.loai_thu_chi_chi_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "loai.thu.chi",
                    ResId = entity.Id.ToString(),
                    Name = $"loai_thu_chi_chi_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}