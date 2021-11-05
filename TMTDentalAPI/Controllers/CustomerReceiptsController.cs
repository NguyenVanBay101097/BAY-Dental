using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReceiptsController : BaseApiController
    {
        private readonly ICustomerReceiptService _customerReceiptService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public CustomerReceiptsController(ICustomerReceiptService customerReceiptService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _customerReceiptService = customerReceiptService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }


        [HttpGet]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Read")]
        public async Task<IActionResult> Get([FromQuery] CustomerReceiptPaged val)
        {
            var result = await _customerReceiptService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _customerReceiptService.GetDisplayById(id);

            var display = _mapper.Map<CustomerReceiptDisplay>(res);

            return Ok(display);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Create")]
        public async Task<IActionResult> Create(CustomerReceiptSave val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            var res = await _customerReceiptService.CreateCustomerReceipt(val);
            var basic = _mapper.Map<CustomerReceiptBasic>(res);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Update")]
        public async Task<IActionResult> Update(Guid id, CustomerReceiptSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = await _customerReceiptService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            await _customerReceiptService.UpdateCustomerReceipt(id, val);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCount(CustomerReceiptGetCountVM val)
        {
            var res = await _customerReceiptService.GetCountToday(val);
            return Ok(res);
        }

        [HttpPatch("{id}/[action]")]
        public async Task<IActionResult> PatchState(Guid id, CustomerReceiptStatePatch result)
        {
            var entity = await _customerReceiptService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var patch = new JsonPatchDocument<CustomerReceiptStatePatch>();
            patch.Replace(x => x.State, result.State);
            patch.Replace(x => x.Reason, result.Reason);
            patch.Replace(x => x.IsNoTreatment, result.IsNoTreatment);
            patch.Replace(x => x.DateExamination, result.DateExamination);
            patch.Replace(x => x.DateDone, result.DateDone);
            var entityMap = _mapper.Map<CustomerReceiptStatePatch>(entity);
            patch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            await _customerReceiptService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var customerReceipt = await _customerReceiptService.GetByIdAsync(id);
            if (customerReceipt == null)
                return NotFound();

            await _customerReceiptService.DeleteAsync(customerReceipt);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\customer_receipt.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample") && (x.Model == "employee" || x.Model == "partner" || x.Model == "sale.order" || x.Model == "product")).ToListAsync();// các irmodel cần thiết
            var entities = await _customerReceiptService.SearchQuery(x => x.DateCreated.Value.Date <= dateToData.Date).Include(x => x.CustomerReceiptProductRels).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<CustomerReceiptXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<CustomerReceiptXmlSampleDataRecord>(entity);

                var partnerModelData = listIrModelData.FirstOrDefault(x => x.ResId == entity.PartnerId.ToString());
                var doctorModelData = listIrModelData.FirstOrDefault(x => x.ResId == entity.DoctorId.ToString());

                item.Id = $@"sample.customer_receipt_{entities.IndexOf(entity) + 1}";
                item.DateRound = (int)(dateToData - entity.DateWaiting.Value).TotalDays;
                item.PartnerId = partnerModelData == null ? "" : partnerModelData?.Module + "." + partnerModelData?.Name;
                item.DoctorId = doctorModelData == null ? "" : doctorModelData?.Module + "." + doctorModelData?.Name;

                //add lines
                foreach (var lineEntity in entity.CustomerReceiptProductRels)
                {
                    var irmodelDataProduct = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductId.ToString());
                    var itemLine = new CustomerReceiptProductRelXmlSampleDataRecord()
                    {
                        ProductId = irmodelDataProduct == null ? "" : irmodelDataProduct?.Module + "." + irmodelDataProduct?.Name
                    };
                    item.CustomerReceiptProductRels.Add(itemLine);
                }

                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "customer.receipt",
                    ResId = entity.Id.ToString(),
                    Name = $"customer_receipt_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}
