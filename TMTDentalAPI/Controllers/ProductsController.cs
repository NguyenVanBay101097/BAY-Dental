using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IApplicationRoleFunctionService _roleFunctionService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IProductStepService _productStepService;
        private readonly IUoMService _uomService;

        public ProductsController(IProductService productService, IMapper mapper,
            IApplicationRoleFunctionService roleFunctionService,
            IProductCategoryService productCategoryService,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService,
            IProductStepService productStepService, IUoMService uomService)
        {
            _productService = productService;
            _mapper = mapper;
            _roleFunctionService = roleFunctionService;
            _productCategoryService = productCategoryService;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
            _productStepService = productStepService;
            _uomService = uomService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ProductPaged val)
        {
            var result = await _productService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _productService.GetProductDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var product = await _productService.CreateProduct(val);

            val.Id = product.Id;

            return Ok(val);
        }

        [HttpPost("CreateWithSteps")]
        public async Task<IActionResult> CreateWithSteps(ProductDisplayAndStep pd)
        {
            if (null == pd || !ModelState.IsValid)
                return BadRequest();

            var val = _mapper.Map<ProductDisplay>(pd);
            var list = pd.stepList;
            
            var product = await _productService.CreateProduct(val);

            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        item.ProductId = product.Id;
                        var productStep = _mapper.Map<ProductStep>(item);

                        await _productStepService.CreateAsync(productStep);
                    }
                }
            }        
            
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _productService.UpdateProduct(id, val);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateLabo(ProductLaboSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var product = await _productService.CreateProduct(val);

            var basic = _mapper.Map<ProductLaboBasic>(product);
            return Ok(basic);
        }

        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> UpdateLabo(Guid id, ProductLaboSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _productService.UpdateProduct(id, val);

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetLabo(Guid id)
        {
            var product = await _productService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            var display = _mapper.Map<ProductLaboDisplay>(product);
            return Ok(display);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLaboPaged([FromQuery]ProductPaged val)
        {
            var result = await _productService.GetLaboPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost("UpdateWithSteps/{id}")]
        public async Task<IActionResult> UpdateWithSteps(Guid id, ProductDisplayAndStep val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _productService.UpdateProduct(id, val);

            //Thêm - cập nhật công đoạn
            var list = val.stepList;
            foreach (var item in list)
            {
                var pdStep = await _productStepService.GetByIdAsync(item.Id);
                if (pdStep == null)
                {
                    item.ProductId = id;
                    pdStep = _mapper.Map(item, pdStep);
                    await _productStepService.CreateAsync(pdStep);
                }
                else
                {
                    pdStep = _mapper.Map(item, pdStep);
                    await _productStepService.UpdateAsync(pdStep);
                }
            }

            //Xóa công đoạn
            var res = _productStepService.GetByProductId(id).Where(x=>!list.Any(y=>y.Id==x.Id || x.Name==y.Name));
            foreach(var item in res)
            {
                var pdStep = await _productStepService.GetByIdAsync(item.Id);
                await _productStepService.DeleteAsync(pdStep);
            }


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            try
            {
                await _productService.DeleteAsync(product);
            }
            catch
            {
                product.Active = false;
                await _productService.UpdateAsync(product);
            }

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _productService.DefaultGet();
            return Ok(res);
        }

        [HttpPost("DefaultProductStepGet")]
        public async Task<IActionResult> DefaultProductStepGet()
        {
            var res = await _productService.DefaultProductStepGet();
            return Ok(res);
        }

        [HttpGet("Autocomplete")]
        public async Task<IActionResult> Autocomplete(string filter = "")
        {
            var res = await _productService.GetProductsAutocomplete(filter: filter);
            return Ok(res);
        }

        [HttpPost("Autocomplete2")]
        public async Task<IActionResult> Autocomplete2(ProductPaged val)
        {
            var res = await _productService.GetProductsAutocomplete2(val);
            return Ok(res);
        }

        [HttpPost("ImportExcel")]
        public async Task<IActionResult> ImportExcel(ProductImportExcelViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for(var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        var categName = Convert.ToString(worksheet.Cells[row, 4].Value);

                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên sản phẩm là bắt buộc");
                        if (string.IsNullOrEmpty(categName))
                            errs.Add("Nhóm sản phẩm là bắt buộc");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }
                      
                        if (!categDict.ContainsKey(categName))
                        {
                            var categ = await _productCategoryService.SearchQuery(x => x.Name == categName && x.Type == val.Type2).FirstOrDefaultAsync();
                            if (categ == null)
                                categ = await _productCategoryService.CreateAsync(new ProductCategory { Name = categName, Type = val.Type2 });
                            categDict.Add(categName, categ);
                        }

                        var item = new ProductImportExcelRow
                        {
                            Name = name,
                            SaleOK = Convert.ToBoolean(worksheet.Cells[row, 2].Value),
                            PurchaseOK = Convert.ToBoolean(worksheet.Cells[row, 3].Value),
                            CategName = categName,
                            DefaultCode = Convert.ToString(worksheet.Cells[row, 5].Value),
                            ListPrice = Convert.ToDecimal(worksheet.Cells[row, 6].Value),
                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var vals = new List<Product>();
            var uom = await _uomService.DefaultUOM();
            foreach (var item in data)
            {
                var pd = new Product();
                pd.CompanyId = CompanyId;
                pd.UOMId = uom.Id;
                pd.UOMPOId = uom.Id;
                pd.Name = item.Name;
                pd.SaleOK = item.SaleOK;
                pd.PurchaseOK = item.PurchaseOK;
                pd.Type = val.Type;
                pd.Type2 = val.Type2;
                pd.CategId = categDict[item.CategName].Id;
                pd.DefaultCode = item.DefaultCode;
                pd.ListPrice = item.ListPrice;
                vals.Add(pd);
            }

            await _productService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }
    }
}