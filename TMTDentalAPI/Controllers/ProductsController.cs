using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
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

        [HttpGet][CheckAccess(Actions = "Catalog.Product.Read")]
        public async Task<IActionResult> Get([FromQuery]ProductPaged val)
        {
            var result = await _productService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")][CheckAccess(Actions = "Catalog.Product.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _productService.GetProductDisplay(id);
            return Ok(res);
        }

        [HttpPost][CheckAccess(Actions = "Catalog.Product.Create")]
        public async Task<IActionResult> Create(ProductSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var product = await _productService.CreateProduct(val);
            _unitOfWork.Commit();

             var res = _mapper.Map<ProductBasic>(product);
            return Ok(res);
        }

        [HttpPut("{id}")][CheckAccess(Actions = "Catalog.Product.Update")]
        public async Task<IActionResult> Update(Guid id, ProductSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _productService.UpdateProduct(id, val);
            _unitOfWork.Commit();

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

        [HttpDelete("{id}")][CheckAccess(Actions = "Catalog.Product.Delete")]
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

        [HttpGet("Autocomplete")][CheckAccess(Actions = "Catalog.Product.Read")]
        public async Task<IActionResult> Autocomplete(string filter = "")
        {
            var res = await _productService.GetProductsAutocomplete(filter: filter);
            return Ok(res);
        }

        [HttpPost("Autocomplete2")][CheckAccess(Actions = "Catalog.Product.Read")]
        public async Task<IActionResult> Autocomplete2(ProductPaged val)
        {
            var res = await _productService.GetProductsAutocomplete2(val);
            return Ok(res);
        }

        [HttpPost("ImportExcel")][CheckAccess(Actions = "Catalog.Product.Create")]
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

            var typeDict = new Dictionary<string, string>()
            {
                { "Dịch vụ", "service" },
                { "Có quản lý tồn kho", "product" },
                { "Không quản lý tồn kho", "consu" }
            };

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for(var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        var categName = Convert.ToString(worksheet.Cells[row, 7].Value);
                        var type = Convert.ToString(worksheet.Cells[row, 6].Value);

                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên sản phẩm là bắt buộc");
                        if (string.IsNullOrEmpty(type))
                            errs.Add("Loại sản phẩm là bắt buộc");
                        if (!string.IsNullOrEmpty(type) && !typeDict.ContainsKey(type))
                            errs.Add($"Loại sản phẩm là không hợp lệ. Giá trị cho phép là {string.Join(", ", typeDict.Keys.ToArray())}");
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
                            IsLabo = Convert.ToBoolean(worksheet.Cells[row, 4].Value),
                            KeToaOK = Convert.ToBoolean(worksheet.Cells[row, 5].Value),
                            Type = type,
                            CategName = categName,
                            DefaultCode = Convert.ToString(worksheet.Cells[row, 8].Value),
                            ListPrice = Convert.ToDecimal(worksheet.Cells[row, 9].Value),
                            PurchasePrice = Convert.ToDecimal(worksheet.Cells[row, 10].Value),
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
                pd.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                pd.SaleOK = item.SaleOK ?? false;
                pd.PurchaseOK = item.PurchaseOK ?? false;
                pd.IsLabo = item.IsLabo ?? false;
                pd.KeToaOK = item.KeToaOK ?? false;
                pd.Type = item.Type;
                pd.Type2 = val.Type2;
                pd.CategId = categDict[item.CategName].Id;
                pd.DefaultCode = item.DefaultCode;
                pd.ListPrice = item.ListPrice ?? 0;
                pd.PurchasePrice = item.PurchasePrice ?? 0;
                vals.Add(pd);
            }

            await _productService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")][CheckAccess(Actions = "Catalog.Product.Create")]
        public async Task<IActionResult> ImportService(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductServiceImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        var categName = Convert.ToString(worksheet.Cells[row, 3].Value);

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
                            var categ = await _productCategoryService.SearchQuery(x => x.Name == categName && x.Type == "service").FirstOrDefaultAsync();
                            if (categ == null)
                                categ = await _productCategoryService.CreateAsync(new ProductCategory { Name = categName, Type = "service" });
                            categDict.Add(categName, categ);
                        }

                        var item = new ProductServiceImportExcelRow
                        {
                            Name = name,
                            IsLabo = Convert.ToBoolean(worksheet.Cells[row, 2].Value),
                            CategName = categName,
                            DefaultCode = Convert.ToString(worksheet.Cells[row, 4].Value),
                            ListPrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value),
                            Steps = Convert.ToString(worksheet.Cells[row, 6].Value),
                            LaboPrice = Convert.ToDecimal(worksheet.Cells[row, 7].Value),
                        };
                        data.Add(item);
                    }
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var uom = await _uomService.DefaultUOM();
            foreach (var item in data)
            {
                var product = !string.IsNullOrEmpty(item.DefaultCode) ? await _productService.SearchQuery(x => x.DefaultCode == item.DefaultCode && x.Type == "service").FirstOrDefaultAsync() : null;
                if (product != null)
                {
                    var res = new ProductSave();
                    res.ListPrice = item.ListPrice ?? 0;
                    res.IsLabo = item.IsLabo ?? false;
                    res.CategId = categDict[item.CategName].Id;
                    res.LaboPrice = item.LaboPrice ?? 0;
                    res.UOMId = product.UOMId;
                    res.UOMPOId = product.UOMPOId;
                    res.Name = item.Name;                   
                    res.SaleOK = product.SaleOK;
                    res.PurchaseOK = product.PurchaseOK;
                    res.Type = product.Type;
                    res.Type2 = product.Type2;                  
                    res.DefaultCode = product.DefaultCode;                  
                    res.PurchasePrice = product.PurchasePrice;
                    if (!string.IsNullOrWhiteSpace(item.Steps))
                    {
                        var stepsArr = item.Steps.Split(";");
                        var order = 1;
                        foreach (var st in stepsArr)
                        {
                            if (string.IsNullOrWhiteSpace(st))
                                continue;
                            res.StepList.ToList().Add(new ProductStepDisplay
                            {
                                Name = st,
                                Order = order++
                            });
                        }
                    }
                    await _productService.UpdateProduct(product.Id,res);
                }
                else
                {
                    var pd = new ProductSave();
                    pd.CompanyId = CompanyId;
                    pd.UOMId = uom.Id;
                    pd.UOMPOId = uom.Id;
                    pd.Name = item.Name;                   
                    pd.SaleOK = true;
                    pd.PurchaseOK = false;
                    pd.IsLabo = item.IsLabo ?? false;
                    pd.Type = "service";
                    pd.Type2 = "service";
                    pd.CategId = categDict[item.CategName].Id;
                    pd.DefaultCode = item.DefaultCode;
                    pd.ListPrice = item.ListPrice ?? 0;
                    pd.LaboPrice = item.LaboPrice ?? 0;
                    pd.PurchasePrice = 0;                  

                    if (!string.IsNullOrWhiteSpace(item.Steps))
                    {
                        var stepsArr = item.Steps.Split(";");
                        var order = 1;
                        foreach (var st in stepsArr)
                        {
                            if (string.IsNullOrWhiteSpace(st))
                                continue;
                            pd.StepList.ToList().Add(new ProductStepDisplay
                            {
                                Name = st,
                                Order = order++
                            });
                        }
                    }
                    await _productService.CreateProduct(pd);
                }
            }

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")][CheckAccess(Actions = "Catalog.Product.Create")]
        public async Task<IActionResult> ImportMedicine(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductMedicineImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        var categName = Convert.ToString(worksheet.Cells[row, 2].Value);

                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên thuốc là bắt buộc");
                        if (string.IsNullOrEmpty(categName))
                            errs.Add("Nhóm thuốc là bắt buộc");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }

                        if (!categDict.ContainsKey(categName))
                        {
                            var categ = await _productCategoryService.SearchQuery(x => x.Name == categName && x.Type == "medicine").FirstOrDefaultAsync();
                            if (categ == null)
                                categ = await _productCategoryService.CreateAsync(new ProductCategory { Name = categName, Type = "medicine" });
                            categDict.Add(categName, categ);
                        }

                        var item = new ProductMedicineImportExcelRow
                        {
                            Name = name,
                            CategName = categName,
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
                pd.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                pd.SaleOK = false;
                pd.PurchaseOK = false;
                pd.KeToaOK = true;
                pd.Type = "consu";
                pd.Type2 = "medicine";
                pd.CategId = categDict[item.CategName].Id;
                pd.ListPrice = 0;
                pd.PurchasePrice = 0;
                vals.Add(pd);
            }

            await _productService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")][CheckAccess(Actions = "Catalog.Product.Create")]
        public async Task<IActionResult> ImportProduct(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductProductImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            var typeDict = new Dictionary<string, string>()
            {
                { "Có quản lý tồn kho", "product" },
                { "Không quản lý tồn kho", "consu" }
            };

            using (var stream = new MemoryStream(fileData))
            {
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        var errs = new List<string>();
                        var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                        var categName = Convert.ToString(worksheet.Cells[row, 3].Value);
                        var type = Convert.ToString(worksheet.Cells[row, 2].Value);

                        if (string.IsNullOrEmpty(name))
                            errs.Add("Tên vật tư là bắt buộc");
                        if (string.IsNullOrEmpty(type))
                            errs.Add("Loại là bắt buộc");
                        if (!string.IsNullOrEmpty(type) && !typeDict.ContainsKey(type))
                            errs.Add($"Loại không hợp lệ. Giá trị cho phép là {string.Join(", ", typeDict.Keys.ToArray())}");
                        if (string.IsNullOrEmpty(categName))
                            errs.Add("Nhóm vật tư là bắt buộc");

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }

                        if (!categDict.ContainsKey(categName))
                        {
                            var categ = await _productCategoryService.SearchQuery(x => x.Name == categName && x.Type == "product").FirstOrDefaultAsync();
                            if (categ == null)
                                categ = await _productCategoryService.CreateAsync(new ProductCategory { Name = categName, Type = "product" });
                            categDict.Add(categName, categ);
                        }

                        var item = new ProductProductImportExcelRow
                        {
                            Name = name,
                            Type = type,
                            CategName = categName,
                            DefaultCode = Convert.ToString(worksheet.Cells[row, 4].Value),
                            PurchasePrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value),
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
                pd.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                pd.SaleOK = false;
                pd.Type = typeDict[item.Type];
                pd.Type2 = "product";
                pd.CategId = categDict[item.CategName].Id;
                pd.DefaultCode = item.DefaultCode;
                pd.PurchasePrice = item.PurchasePrice ?? 0;
                vals.Add(pd);
            }

            await _productService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangeUOM(ProductOnChangeUOM val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var uom = val.UOMId.HasValue ? (await _uomService.GetByIdAsync(val.UOMId)) : null;
            var uom_po = val.UOMPOId.HasValue ? (await _uomService.GetByIdAsync(val.UOMPOId)) : null;
            if (uom != null && uom_po == null)
                uom_po = uom;
            if (uom != null && uom_po != null && uom.CategoryId != uom_po.CategoryId)
                uom_po = uom;

            var res = new ProductOnChangeUOMResponse
            {
                UOM = uom != null ? _mapper.Map<UoMBasic>(uom) : null,
                UOMPO = uom_po != null ? _mapper.Map<UoMBasic>(uom_po) : null
            };

            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetUOMs(Guid id)
        {
            var res = await _mapper.ProjectTo<UoMBasic>(_productService.SearchQuery(x => x.Id == id)
                .SelectMany(x => x.ProductUoMRels).Select(x => x.UoM)).ToListAsync();
            return Ok(res);
        }

        [HttpGet("[action]")][CheckAccess(Actions = "Catalog.Product.Read")]
        public async Task<IActionResult> ExportServiceExcelFile([FromQuery]ProductPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var services = await _productService.GetServiceExportExcel(val);
            //var categDict = new Dictionary<Guid, ProductCategory>();
            var sheetName = "Thông tin dịch vụ ";
            byte[] fileContent;

           
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Tên dịch vụ";
                worksheet.Cells[1, 2].Value = "Có thể đặt labo";
                worksheet.Cells[1, 3].Value = "Nhóm dịch vụ";
                worksheet.Cells[1, 4].Value = "Mã dịch vụ";
                worksheet.Cells[1, 5].Value = "Giá bán";
                worksheet.Cells[1, 6].Value = "Công đoạn";
                worksheet.Cells[1, 7].Value = "Giá đặt labo";              
                for (int row = 2; row < services.Count() + 2; row++)
                {
                    var item = services.ToList()[row - 2];

                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = item.IsLabo;
                    worksheet.Cells[row, 3].Value = item.CategName;
                    worksheet.Cells[row, 4].Value = item.DefaultCode;
                    worksheet.Cells[row, 5].Value = item.ListPrice;
                    worksheet.Cells[row, 6].Value = item.StepList.Count() == 0 ? null : string.Join(";", item.StepList.Select(x=>x.Name).ToList());
                    worksheet.Cells[row, 7].Value = item.PurchasePrice ?? 0;
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