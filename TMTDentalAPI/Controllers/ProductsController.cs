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

        [HttpGet]
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> Get([FromQuery] ProductPaged val)
        {
            var result = await _productService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _productService.GetProductDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.Products.Create")]
        public async Task<IActionResult> Create(ProductSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var product = await _productService.CreateProduct(val);
            _unitOfWork.Commit();

            var res = _mapper.Map<ProductBasic>(product);
            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.Products.Update")]
        public async Task<IActionResult> Update(Guid id, ProductSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _productService.UpdateProduct(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> GetLabo(Guid id)
        {
            var product = await _productService.SearchQuery(x => x.Id == id).Include(x => x.Categ).FirstOrDefaultAsync();
            var display = _mapper.Map<ProductLaboDisplay>(product);
            return Ok(display);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLaboPaged([FromQuery] ProductPaged val)
        {
            var result = await _productService.GetLaboPagedResultAsync(val);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.Products.Delete")]
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
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> Autocomplete(string filter = "")
        {
            var res = await _productService.GetProductsAutocomplete(filter: filter);
            return Ok(res);
        }

        [HttpPost("Autocomplete2")]
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> Autocomplete2(ProductPaged val)
        {
            var res = await _productService.GetProductsAutocomplete2(val);
            return Ok(res);
        }

        [HttpPost("ImportExcel")]
        [CheckAccess(Actions = "Catalog.Products.Create")]
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
                    for (var row = 2; row <= worksheet.Dimension.Rows; row++)
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

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Products.Create")]
        public async Task<IActionResult> ImportService(ProductImportExcelBaseViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductServiceImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var errors = new List<string>();

            try
            {
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

            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Lỗi đọc dữ liệu: {e.Message}" } });
            }

            var categNames = data.Select(x => x.CategName).Distinct().ToList();
            if (categNames.Any())
            {
                var categs = await _productCategoryService.SearchQuery(x => categNames.Contains(x.Name) && x.Type == "service").ToListAsync();
                foreach (var categName in categNames)
                {
                    if (categDict.ContainsKey(categName))
                        continue;

                    var categ = categs.FirstOrDefault(x => x.Name == categName);
                    if (categ == null)
                        categ = await _productCategoryService.CreateAsync(new ProductCategory { Name = categName, Type = "service" });
                    categDict.Add(categName, categ);
                }
            }

            var productDict = new Dictionary<string, Product>();
            var productCodes = data.Where(x => !string.IsNullOrEmpty(x.DefaultCode))
               .Select(x => x.DefaultCode).Distinct().ToList();
            if (productCodes.Any())
            {
                var products = await _productService.SearchQuery(x => productCodes.Contains(x.DefaultCode) && x.Type == "service")
                    .Include(x => x.Steps).ToListAsync();
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.DefaultCode) || productDict.ContainsKey(product.DefaultCode))
                        continue;
                    productDict.Add(product.DefaultCode, product);
                }
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var uom = await _uomService.DefaultUOM();
            var productsCreate = new List<Product>();
            var productsUpdate = new List<Product>();

            foreach (var item in data)
            {
                var product = !string.IsNullOrEmpty(item.DefaultCode) && productDict.ContainsKey(item.DefaultCode) ?
                    productDict[item.DefaultCode] : null;
                if (product == null)
                {
                    product = new Product();
                    product.CompanyId = CompanyId;
                    product.ListPrice = item.ListPrice ?? 0;
                    product.IsLabo = item.IsLabo ?? false;
                    product.CategId = categDict[item.CategName].Id;
                    product.LaboPrice = item.LaboPrice ?? 0;
                    product.UOMId = uom.Id;
                    product.UOMPOId = uom.Id;
                    product.Name = item.Name;
                    product.SaleOK = true;
                    product.PurchaseOK = false;
                    product.Type = "service";
                    product.Type2 = "service";
                    product.DefaultCode = item.DefaultCode;
                    if (!string.IsNullOrWhiteSpace(item.Steps))
                    {
                        var stepsArr = item.Steps.Split(";");
                        var order = 1;
                        foreach (var st in stepsArr)
                        {
                            if (string.IsNullOrWhiteSpace(st))
                                continue;
                            product.Steps.Add(new ProductStep
                            {
                                Name = st,
                                Order = order++
                            });
                        }
                    }

                    productsCreate.Add(product);
                }
                else
                {
                    product.Name = item.Name;
                    product.SaleOK = true;
                    product.PurchaseOK = false;
                    product.IsLabo = item.IsLabo ?? false;
                    product.CategId = categDict[item.CategName].Id;
                    product.DefaultCode = item.DefaultCode;
                    product.ListPrice = item.ListPrice ?? 0;
                    product.LaboPrice = item.LaboPrice ?? 0;
                    product.Steps.Clear();
                    if (!string.IsNullOrWhiteSpace(item.Steps))
                    {
                        var stepsArr = item.Steps.Split(";");
                        var order = 1;
                        foreach (var st in stepsArr)
                        {
                            if (string.IsNullOrWhiteSpace(st))
                                continue;
                            product.Steps.Add(new ProductStep
                            {
                                Name = st,
                                Order = order++
                            });
                        }
                    }

                    productsUpdate.Add(product);
                }
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _productService.CreateAsync(productsCreate);
                await _productService.UpdateAsync(productsUpdate);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Lưu thất bại: {e.Message}" } });
            }

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Products.Create")]
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

            try
            {
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
                            var price = Convert.ToString(worksheet.Cells[row, 3].Value);
                            var defaultCode = Convert.ToString(worksheet.Cells[row, 5].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên thuốc là bắt buộc");
                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm thuốc là bắt buộc");
                            if (string.IsNullOrEmpty(price))
                                errs.Add("Giá thuốc là bắt buộc");

                            //tìm exist đơn vị tính và gán Id uom cho thuốc
                            var uomName = Convert.ToString(worksheet.Cells[row, 4].Value).Trim();
                            if (string.IsNullOrEmpty(uomName))
                                errs.Add("Đơn vị mặc định là bắt buộc và phải tồn tại trong hệ thống");
                            var uom = _uomService.SearchQuery(x => x.Name.Trim().ToLower().Contains(uomName.ToLower())).FirstOrDefault();
                            if (uom == null)
                                errs.Add("Đơn vị mặc định là bắt buộc và phải tồn tại trong hệ thống");

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
                                ListPrice = Convert.ToDecimal(worksheet.Cells[row, 3].Value),
                                UomId = uom.Id,
                                DefaultCode = defaultCode
                            };
                            data.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Lỗi đọc dữ liệu: {e.Message}" } });
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var categNames = data.Select(x => x.CategName).Distinct().ToList();
            if (categNames.Any())
            {
                var categs = await _productCategoryService.SearchQuery(x => categNames.Contains(x.Name) && x.Type == "medicine").ToListAsync();
                foreach (var categName in categNames)
                {
                    if (categDict.ContainsKey(categName))
                        continue;

                    var categ = categs.FirstOrDefault(x => x.Name == categName);
                    if (categ == null)
                        categ = await _productCategoryService.CreateAsync(new ProductCategory { Name = categName, Type = "medicine" });
                    categDict.Add(categName, categ);
                }
            }

            var productDict = new Dictionary<string, Product>();
            var productCodes = data.Where(x => !string.IsNullOrEmpty(x.DefaultCode))
               .Select(x => x.DefaultCode).Distinct().ToList();
            if (productCodes.Any())
            {
                var products = await _productService.SearchQuery(x => productCodes.Contains(x.DefaultCode) && x.Type2 == "medicine").ToListAsync();

                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.DefaultCode) || productDict.ContainsKey(product.DefaultCode))
                        continue;
                    productDict.Add(product.DefaultCode, product);
                }
            }

            var productsCreate = new List<Product>();
            var productsUpdate = new List<Product>();
            //var uom = await _uomService.DefaultUOM();
            foreach (var item in data)
            {
                var product = !string.IsNullOrEmpty(item.DefaultCode) && productDict.ContainsKey(item.DefaultCode) ? productDict[item.DefaultCode] : null;
                if (product == null)
                {
                    var pd = new Product();
                    pd.CompanyId = CompanyId;
                    pd.UOMId = item.UomId.Value;
                    pd.UOMPOId = item.UomId.Value;
                    pd.Name = item.Name;
                    pd.SaleOK = false;
                    pd.PurchaseOK = false;
                    pd.KeToaOK = true;
                    pd.Type = "consu";
                    pd.Type2 = "medicine";
                    pd.CategId = categDict[item.CategName].Id;
                    pd.ListPrice = item.ListPrice;
                    pd.PurchasePrice = 0;
                    productsCreate.Add(pd);
                }
                else
                {
                    product.Name = item.Name;
                    product.CategId = categDict[item.CategName].Id;
                    product.UOMId = item.UomId.Value;
                    product.UOMPOId = item.UomId.Value;
                    product.ListPrice = item.ListPrice;
                    productsUpdate.Add(product);
                }
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _productService.CreateAsync(productsCreate);
                await _productService.UpdateAsync(productsUpdate);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Lưu thất bại: {e.Message}" } });
            }

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Products.Create")]
        public async Task<IActionResult> ImportProduct(ProductImportExcelBaseViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductProductImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var productDict = new Dictionary<string, Product>();
            var errors = new List<string>();

            var typeDict = new Dictionary<string, string>()
            {
                { "Có quản lý tồn kho", "product" },
                { "Không quản lý tồn kho", "consu" }
            };

            try
            {
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
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Lỗi đọc dữ liệu: {e.Message}" } });
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var categNames = data.Select(x => x.CategName).Distinct().ToList();
            if (categNames.Any())
            {
                var categs = await _productCategoryService.SearchQuery(x => categNames.Contains(x.Name) && x.Type == "product").ToListAsync();
                foreach (var categName in categNames)
                {
                    if (categDict.ContainsKey(categName))
                        continue;

                    var categ = categs.FirstOrDefault(x => x.Name == categName);
                    if (categ == null)
                        categ = await _productCategoryService.CreateAsync(new ProductCategory { Name = categName, Type = "product" });
                    categDict.Add(categName, categ);
                }
            }

            var productCodes = data.Where(x => !string.IsNullOrEmpty(x.DefaultCode))
                .Select(x => x.DefaultCode).Distinct().ToList();
            if (productCodes.Any())
            {
                var products = await _productService.SearchQuery(x => productCodes.Contains(x.DefaultCode) && x.Type == "product").ToListAsync();
                foreach (var product in products)
                {
                    if (string.IsNullOrEmpty(product.DefaultCode) || productDict.ContainsKey(product.DefaultCode))
                        continue;
                    productDict.Add(product.DefaultCode, product);
                }
            }

            var uom = await _uomService.DefaultUOM();
            var productsCreate = new List<Product>();
            var productsUpdate = new List<Product>();
            foreach (var item in data)
            {
                var product = !string.IsNullOrEmpty(item.DefaultCode) && productDict.ContainsKey(item.DefaultCode) ?
                    productDict[item.DefaultCode] : null;
                if (product == null)
                {
                    product = new Product();
                    product.CompanyId = CompanyId;
                    product.UOMId = uom.Id;
                    product.UOMPOId = uom.Id;
                    product.Name = item.Name;
                    product.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                    product.SaleOK = false;
                    product.Type = typeDict[item.Type];
                    product.Type2 = "product";
                    product.CategId = categDict[item.CategName].Id;
                    product.DefaultCode = item.DefaultCode;
                    product.PurchasePrice = item.PurchasePrice ?? 0;

                    productsCreate.Add(product);
                }
                else
                {
                    product.Name = item.Name;
                    product.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                    product.CategId = categDict[item.CategName].Id;
                    product.DefaultCode = item.DefaultCode;
                    product.PurchasePrice = item.PurchasePrice ?? 0;

                    productsUpdate.Add(product);
                }
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _productService.CreateAsync(productsCreate);
                await _productService.UpdateAsync(productsUpdate);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Lưu thất bại: {e.Message}" } });
            }

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Products.Create")]
        public async Task<IActionResult> ImportLabo(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductLaboImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                try
                {
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var errs = new List<string>();
                            var name = Convert.ToString(worksheet.Cells[row, 1].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên vật liệu Labo là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }

                            var item = new ProductLaboImportExcelRow
                            {
                                Name = name,
                            };
                            data.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception("Dữ liệu file không đúng định dạng mẫu");
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
                pd.KeToaOK = false;
                pd.Type = "consu";
                pd.Type2 = "labo";
                pd.ListPrice = 0;
                pd.PurchasePrice = 0;
                vals.Add(pd);
            }

            await _productService.CreateAsync(vals);

            _unitOfWork.Commit();

            return Ok(new { success = true });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.Products.Create")]
        public async Task<IActionResult> ImportLaboAttach(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductLaboImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var errors = new List<string>();
            await _unitOfWork.BeginTransactionAsync();

            using (var stream = new MemoryStream(fileData))
            {
                try
                {
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        for (var row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            var errs = new List<string>();
                            var name = Convert.ToString(worksheet.Cells[row, 1].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên gửi kèm Labo là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }

                            var item = new ProductLaboImportExcelRow
                            {
                                Name = name,
                            };
                            data.Add(item);
                        }
                    }
                }
                catch (Exception)
                {

                    throw new Exception("Dữ liệu file không đúng định dạng mẫu");
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
                pd.KeToaOK = false;
                pd.Type = "consu";
                pd.Type2 = "labo_attach";
                pd.ListPrice = 0;
                pd.PurchasePrice = 0;
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

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> ExportServiceExcelFile([FromQuery] ProductPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var services = await _productService.GetServiceExportExcel(val);
            var sheetName = "Dịch vụ";
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
                    worksheet.Cells[row, 6].Value = item.StepList.Count() == 0 ? null : string.Join(";", item.StepList.Select(x => x.Name).ToList());
                    worksheet.Cells[row, 7].Value = item.PurchasePrice ?? 0;
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
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> ExportProductExcel(ProductPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var products = await _productService.GetProductExportExcel(val);
            var sheetName = "Vật tư";
            byte[] fileContent;
            var type_dict = new Dictionary<string, string>()
            {
                { "product", "Có quản lý tồn kho" },
                { "consu", "Không quản lý tồn kho" }
            };

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Tên vật tư";
                worksheet.Cells[1, 2].Value = "Loại";
                worksheet.Cells[1, 3].Value = "Nhóm vật tư";
                worksheet.Cells[1, 4].Value = "Mã vật tư";
                worksheet.Cells[1, 5].Value = "Giá nhập";

                var row = 2;
                foreach (var item in products)
                {
                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = type_dict[item.Type];
                    worksheet.Cells[row, 3].Value = item.CategName;
                    worksheet.Cells[row, 4].Value = item.DefaultCode;
                    worksheet.Cells[row, 5].Value = item.PurchasePrice ?? 0;

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
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> ExportMedicineExcel(ProductPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var products = await _productService.GetMedicineExportExcel(val);
            var sheetName = "thuốc";
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Tên thuốc";
                worksheet.Cells[1, 2].Value = "Nhóm thuốc";
                worksheet.Cells[1, 3].Value = "Giá thuốc";
                worksheet.Cells[1, 4].Value = "Đơn vị tính mặc định";
                worksheet.Cells[1, 5].Value = "Mã thuốc";

                var row = 2;
                foreach (var item in products)
                {
                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = item.CategName;
                    worksheet.Cells[row, 3].Value = item.ListPrice;
                    worksheet.Cells[row, 4].Value = item.UomName;
                    worksheet.Cells[row, 5].Value = item.DefaultCode;
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
    }
}