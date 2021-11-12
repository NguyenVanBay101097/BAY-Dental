﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IIRModelDataService _modelDataService;
        private readonly IProductStepService _productStepService;
        private readonly IUoMService _uomService;
        private readonly IUoMCategoryService _uomCategService;

        public ProductsController(IProductService productService, IMapper mapper,
            IApplicationRoleFunctionService roleFunctionService,
            IProductCategoryService productCategoryService,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService,
            IIRModelDataService modelDataService,
            IProductStepService productStepService, IUoMService uomService,
            IUoMCategoryService uomCategService)
        {
            _productService = productService;
            _mapper = mapper;
            _roleFunctionService = roleFunctionService;
            _productCategoryService = productCategoryService;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
            _modelDataService = modelDataService;
            _productStepService = productStepService;
            _uomService = uomService;
            _uomCategService = uomCategService;
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

        //Lấy danh sách những product mà tồn kho hiện tại nhỏ hơn tồn kho tối thiểu được thiết lập
        [HttpGet("[action]")]
        public async Task<IActionResult> GetProductsComingEnd([FromQuery] ProductGetProductsComingEndRequest val)
        {
            var results = await _productService.GetProductsComingEnd(val);
            return Ok(results);
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

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultProducMedicine()
        {
            var res = await _productService.GetDefaultProductMedicine();
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

        [HttpPost("Autocomplete3")]
        [CheckAccess(Actions = "Catalog.Products.Read")]
        public async Task<IActionResult> Autocomplete3(ProductPaged val)
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

            _productService._ComputeUoMRels(vals);
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
            var standardPriceDict = new Dictionary<string, decimal?>();
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
                            var name = Convert.ToString(worksheet.Cells[row, 2].Value);
                            var categName = Convert.ToString(worksheet.Cells[row, 4].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên dịch vụ là bắt buộc");
                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm dịch vụ là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }

                            var item = new ProductServiceImportExcelRow
                            {
                                Name = name,
                                DefaultCode = Convert.ToString(worksheet.Cells[row, 1].Value),
                                IsLabo = Convert.ToBoolean(worksheet.Cells[row, 3].Value),
                                CategName = categName,
                                ListPrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value),
                                StandardPrice = Convert.ToDecimal(worksheet.Cells[row, 6].Value),
                                Steps = Convert.ToString(worksheet.Cells[row, 7].Value),
                                LaboPrice = Convert.ToDecimal(worksheet.Cells[row, 8].Value),
                                Firm = Convert.ToString(worksheet.Cells[row, 9].Value),
                            };
                            data.Add(item);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng" } });
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

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

            var standardPrices = data.Select(x => new
            {
                x.Name,
                x.StandardPrice
            }).ToList();

            foreach (var standardPrice in standardPrices)
            {
                standardPriceDict.Add(standardPrice.Name, standardPrice?.StandardPrice);
            }
            var productDict = new Dictionary<string, Product>();

            var uom = await _uomService.DefaultUOM();
            var productsCreate = new List<Product>();

            foreach (var item in data)
            {
                var product = new Product();
                product.CompanyId = CompanyId;
                product.ListPrice = item.ListPrice ?? 0;
                product.IsLabo = item.IsLabo ?? false;
                product.Firm = item.Firm;
                product.CategId = categDict[item.CategName].Id;
                product.LaboPrice = item.LaboPrice ?? 0;
                product.UOMId = uom.Id;
                product.UOMPOId = uom.Id;
                product.Name = item.Name;
                product.DefaultCode = item.DefaultCode;
                product.SaleOK = true;
                product.PurchaseOK = false;
                product.Type = "service";
                product.Type2 = "service";
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

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _productService._ComputeUoMRels(productsCreate);
                var products = await _productService.CreateAsync(productsCreate);

                foreach (var product in products)
                {
                    _productService.SetStandardPrice(product, Convert.ToDouble(standardPriceDict[product.Name]), product.CompanyId);
                }

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
        public async Task<IActionResult> UpdateServiceFromExcel(ProductImportExcelBaseViewModel val)
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
                            var serviceCode = Convert.ToString(worksheet.Cells[row, 4].Value);

                            if (string.IsNullOrEmpty(serviceCode))
                                errs.Add("Mã dịch vụ là bắt buộc");

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên dịch vụ là bắt buộc");

                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm dịch vụ là bắt buộc");

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
                                DefaultCode = serviceCode,
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
                return Ok(new { success = false, errors = new List<string>() { $"File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng" } });
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

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

            var productsUpdate = new List<Product>();
            var productCodes = data.Select(x => x.DefaultCode).Distinct().ToList();
            var productsToUpdate = await _productService.SearchQuery(x => x.Type2 == "service" && productCodes.Contains(x.DefaultCode))
                .Include(x => x.Steps)
                .ToListAsync();
            var productDict = productsToUpdate.GroupBy(x => x.DefaultCode).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var notFoundProductCodes = productCodes.Where(x => !productDict.ContainsKey(x)).ToList();
            if (notFoundProductCodes.Any())
                return Ok(new { success = false, errors = new List<string>() { $"Mã dịch vụ không tồn tại: {string.Join(", ", notFoundProductCodes)}" } });

            foreach (var item in data)
            {
                if (!productDict.ContainsKey(item.DefaultCode))
                    continue;

                var product = productDict[item.DefaultCode];
                if (product == null)
                    continue;

                product.Name = item.Name;
                product.IsLabo = item.IsLabo ?? false;
                product.CategId = categDict[item.CategName].Id;
                product.ListPrice = item.ListPrice ?? 0;
                product.LaboPrice = item.LaboPrice ?? 0;

                productsUpdate.Add(product);
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
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _productService._ComputeUoMRels(productsUpdate);
                await _productService.UpdateAsync(productsUpdate);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Cập nhật thất bại: {e.Message}" } });
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
            var uomDict = new Dictionary<string, UoM>();
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
                            var price = Convert.ToString(worksheet.Cells[row, 4].Value);
                            //var defaultCode = Convert.ToString(worksheet.Cells[row, 5].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên thuốc là bắt buộc");
                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm thuốc là bắt buộc");
                            if (string.IsNullOrEmpty(price))
                                errs.Add("Giá thuốc là bắt buộc");

                            var uomName = Convert.ToString(worksheet.Cells[row, 5].Value).Trim();
                            if (string.IsNullOrEmpty(uomName))
                                errs.Add("Đơn vị mặc định là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                            var minInventory = worksheet.Cells[row, 6].Value;
                            var origin = worksheet.Cells[row, 7].Value;
                            var expiry = worksheet.Cells[row, 8].Value;
                            var item = new ProductMedicineImportExcelRow
                            {
                                Name = name,
                                DefaultCode = Convert.ToString(worksheet.Cells[row, 2].Value),
                                CategName = categName,
                                ListPrice = Convert.ToDecimal(worksheet.Cells[row, 4].Value),
                                UoM = uomName,
                                MinInventory = minInventory != null ? Convert.ToDecimal(minInventory) : (decimal?)minInventory,
                                Origin = origin != null ? Convert.ToString(origin) : null,
                                Expiry = expiry != null ? Convert.ToDecimal(expiry) : (decimal?)expiry
                            };
                            data.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng" } });
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

            var uomNames = data.Select(x => x.UoM).Distinct().ToList();
            if (uomNames.Any())
            {
                var uoms = await _uomService.SearchQuery(x => uomNames.Contains(x.Name) && x.UOMType == "reference").ToListAsync();
                foreach (var uomName in uomNames)
                {
                    if (categDict.ContainsKey(uomName))
                        continue;

                    var uom = uoms.FirstOrDefault(x => x.Name == uomName);
                    if (uom == null)
                    {
                        var uomCateg = await _uomCategService.SearchQuery(x => x.MeasureType == "unit").FirstOrDefaultAsync();
                        uom = await _uomService.CreateAsync(new UoM { Name = uomName, CategoryId = uomCateg.Id });
                    }
                    uomDict.Add(uomName, uom);
                }
            }

            var vals = new List<Product>();
            foreach (var item in data)
            {
                var pd = new Product();
                pd.CompanyId = CompanyId;
                pd.UOMId = uomDict[item.UoM].Id;
                pd.UOMPOId = uomDict[item.UoM].Id;
                pd.Name = item.Name;
                pd.DefaultCode = item.DefaultCode;
                pd.SaleOK = false;
                pd.PurchaseOK = false;
                pd.KeToaOK = true;
                pd.Type = "consu";
                pd.Type2 = "medicine";
                pd.CategId = categDict[item.CategName].Id;
                pd.ListPrice = item.ListPrice;
                pd.PurchasePrice = 0;
                pd.MinInventory = item.MinInventory;
                pd.Origin = item.Origin;
                pd.Expiry = item.Expiry;
                vals.Add(pd);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _productService._ComputeUoMRels(vals);
                await _productService.CreateAsync(vals);


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
        public async Task<IActionResult> UpdateMedicineFromExcel(ProductImportExcelBaseViewModel val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductMedicineUpdateExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
            var uomDict = new Dictionary<string, UoM>();
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
                            var price = Convert.ToString(worksheet.Cells[row, 4].Value);
                            var defaultCode = Convert.ToString(worksheet.Cells[row, 2].Value);

                            if (string.IsNullOrEmpty(defaultCode))
                                errs.Add("Mã thuốc là bắt buộc"); ;
                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên thuốc là bắt buộc");
                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm thuốc là bắt buộc");
                            if (string.IsNullOrEmpty(price))
                                errs.Add("Giá thuốc là bắt buộc");

                            //tìm exist đơn vị tính và gán Id uom cho thuốc
                            var uomName = Convert.ToString(worksheet.Cells[row, 5].Value).Trim();
                            if (string.IsNullOrEmpty(uomName))
                                errs.Add("Đơn vị mặc định là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                            var minInventory = worksheet.Cells[row, 6].Value;
                            var origin = worksheet.Cells[row, 7].Value;
                            var expiry = worksheet.Cells[row, 8].Value;
                            var item = new ProductMedicineUpdateExcelRow
                            {
                                Name = name,
                                CategName = categName,
                                ListPrice = Convert.ToDecimal(worksheet.Cells[row, 4].Value),
                                UoM = uomName,
                                DefaultCode = defaultCode,
                                MinInventory = minInventory != null ? Convert.ToDecimal(minInventory) : (decimal?)minInventory,
                                Origin = origin != null ? Convert.ToString(origin) : null,
                                Expiry = expiry != null ? Convert.ToDecimal(expiry) : (decimal?)expiry

                            };
                            data.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng" } });
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

            var uomNames = data.Select(x => x.UoM).Distinct().ToList();
            if (uomNames.Any())
            {
                var uoms = await _uomService.SearchQuery(x => uomNames.Contains(x.Name) && x.UOMType == "reference").ToListAsync();
                foreach (var uomName in uomNames)
                {
                    if (uomDict.ContainsKey(uomName))
                        continue;

                    var uom = uoms.FirstOrDefault(x => x.Name == uomName);
                    if (uom == null)
                    {
                        var uomCateg = await _uomCategService.SearchQuery(x => x.MeasureType == "unit").FirstOrDefaultAsync();
                        uom = await _uomService.CreateAsync(new UoM { Name = uomName, CategoryId = uomCateg.Id });
                    }

                    uomDict.Add(uomName, uom);
                }
            }

            var productsUpdate = new List<Product>();
            var productCodes = data.Select(x => x.DefaultCode).Distinct().ToList();
            var productsToUpdate = await _productService.SearchQuery(x => x.Type2 == "medicine" && productCodes.Contains(x.DefaultCode))
                .Include(x => x.ProductUoMRels)
                .ToListAsync();
            var productDict = productsToUpdate.GroupBy(x => x.DefaultCode).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var notFoundProductCodes = productCodes.Where(x => !productDict.ContainsKey(x)).ToList();
            if (notFoundProductCodes.Any())
                return Ok(new { success = false, errors = new List<string>() { $"Mã thuốc không tồn tại: {string.Join(", ", notFoundProductCodes)}" } });

            foreach (var item in data)
            {
                if (!productDict.ContainsKey(item.DefaultCode))
                    continue;

                var product = productDict[item.DefaultCode];
                product.Name = item.Name;
                product.CategId = categDict[item.CategName].Id;
                product.UOMId = uomDict[item.UoM].Id;
                product.UOMPOId = uomDict[item.UoM].Id;
                product.ListPrice = item.ListPrice;
                product.Origin = item.Origin;
                product.Expiry = item.Expiry;
                productsUpdate.Add(product);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _productService._ComputeUoMRels(productsUpdate);
                await _productService.UpdateAsync(productsUpdate);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Cập nhật thất bại: {e.Message}" } });
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
                            var categName = Convert.ToString(worksheet.Cells[row, 4].Value);
                            var type = Convert.ToString(worksheet.Cells[row, 3].Value);
                            var uomName = Convert.ToString(worksheet.Cells[row, 6].Value);
                            var uomPOName = Convert.ToString(worksheet.Cells[row, 7].Value);

                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên vật tư là bắt buộc");
                            if (string.IsNullOrEmpty(type))
                                errs.Add("Loại là bắt buộc");
                            if (string.IsNullOrEmpty(uomName))
                                errs.Add("Đơn vị mặc định là bắt buộc");
                            if (string.IsNullOrEmpty(uomPOName))
                                errs.Add("Đơn vị mua là bắt buộc");

                            if (!string.IsNullOrEmpty(type) && !typeDict.ContainsKey(type))
                                errs.Add($"Loại không hợp lệ. Giá trị cho phép là {string.Join(", ", typeDict.Keys.ToArray())}");
                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm vật tư là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                            var minInventory = worksheet.Cells[row, 8].Value;
                            var origin = worksheet.Cells[row, 9].Value;
                            var expiry = worksheet.Cells[row, 10].Value;
                            var item = new ProductProductImportExcelRow
                            {
                                Name = name,
                                DefaultCode = Convert.ToString(worksheet.Cells[row, 2].Value),
                                Type = type,
                                CategName = categName,
                                PurchasePrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value),
                                UOM = uomName,
                                UOMPO = uomPOName,
                                MinInventory = minInventory != null ? Convert.ToDecimal(minInventory) : (decimal?)minInventory,
                                Origin = origin != null ? Convert.ToString(origin) : null,
                                Expiry = expiry != null ? Convert.ToDecimal(expiry) : (decimal?)expiry
                            };
                            data.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng" } });
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var uomNames = data.Select(x => x.UOM).Union(data.Select(x => x.UOMPO)).Distinct().ToList();
            var uoms = await _uomService.SearchQuery(x => uomNames.Contains(x.Name)).ToListAsync();
            var uomDict = uoms.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var notFoundUomNames = uomNames.Where(x => !uomDict.ContainsKey(x)).ToList();
            if (notFoundUomNames.Any())
                return Ok(new { success = false, errors = new List<string>() { $"Đơn vị tính không tồn tại: {string.Join(", ", notFoundUomNames)}" } });

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

            var productsCreate = new List<Product>();
            foreach (var item in data)
            {
                var uom = uomDict[item.UOM];
                var uomPO = uomDict[item.UOMPO];
                var product = new Product();
                product.CompanyId = CompanyId;
                product.UOMId = uom.Id;
                product.UOMPOId = uomPO.Id;
                product.Name = item.Name;
                product.DefaultCode = item.DefaultCode;
                product.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                product.SaleOK = false;
                product.Type = typeDict[item.Type];
                product.Type2 = "product";
                product.CategId = categDict[item.CategName].Id;
                product.PurchasePrice = item.PurchasePrice ?? 0;
                product.MinInventory = item.MinInventory ?? 0;
                product.Origin = item.Origin;
                product.Expiry = item.Expiry;
                product.ProductUoMRels.Add(new ProductUoMRel { UoMId = uom.Id });
                if (uom.Id != uomPO.Id)
                    product.ProductUoMRels.Add(new ProductUoMRel { UoMId = uomPO.Id });

                productsCreate.Add(product);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _productService._ComputeUoMRels(productsCreate);
                await _productService.CreateAsync(productsCreate);
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
        public async Task<IActionResult> UpdateProductFromExcel(ProductImportExcelBaseViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ProductProductImportExcelRow>();
            var categDict = new Dictionary<string, ProductCategory>();
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
                            var productCode = Convert.ToString(worksheet.Cells[row, 2].Value);
                            var name = Convert.ToString(worksheet.Cells[row, 1].Value);
                            var type = Convert.ToString(worksheet.Cells[row, 3].Value);
                            var categName = Convert.ToString(worksheet.Cells[row, 4].Value);
                            var uomName = Convert.ToString(worksheet.Cells[row, 6].Value);
                            var uomPOName = Convert.ToString(worksheet.Cells[row, 7].Value);

                            if (string.IsNullOrEmpty(productCode))
                                errs.Add("Mã vật tư là bắt buộc"); ;
                            if (string.IsNullOrEmpty(name))
                                errs.Add("Tên vật tư là bắt buộc");
                            if (string.IsNullOrEmpty(type))
                                errs.Add("Loại là bắt buộc");
                            if (string.IsNullOrEmpty(uomName))
                                errs.Add("Đơn vị mặc định là bắt buộc");
                            if (string.IsNullOrEmpty(uomPOName))
                                errs.Add("Đơn vị mua là bắt buộc");
                            if (!string.IsNullOrEmpty(type) && !typeDict.ContainsKey(type))
                                errs.Add($"Loại không hợp lệ. Giá trị cho phép là {string.Join(", ", typeDict.Keys.ToArray())}");
                            if (string.IsNullOrEmpty(categName))
                                errs.Add("Nhóm vật tư là bắt buộc");

                            if (errs.Any())
                            {
                                errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                                continue;
                            }
                            var minInventory = worksheet.Cells[row, 8].Value;
                            var origin = worksheet.Cells[row, 9].Value;
                            var expiry = worksheet.Cells[row, 10].Value;
                            var item = new ProductProductImportExcelRow
                            {
                                Name = name,
                                Type = type,
                                CategName = categName,
                                DefaultCode = productCode,
                                PurchasePrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value),
                                UOM = uomName,
                                UOMPO = uomPOName,
                                MinInventory = minInventory != null ? Convert.ToDecimal(minInventory) : (decimal?)minInventory,
                                Origin = origin != null ? Convert.ToString(origin) : null,
                                Expiry = expiry != null ? Convert.ToDecimal(expiry) : (decimal?)expiry
                            };
                            data.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng" } });
            }

            if (errors.Any())
                return Ok(new { success = false, errors });

            var uomNames = data.Select(x => x.UOM).Union(data.Select(x => x.UOMPO)).Distinct().ToList();
            var uoms = await _uomService.SearchQuery(x => uomNames.Contains(x.Name)).ToListAsync();
            var uomDict = uoms.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var notFoundUomNames = uomNames.Where(x => !uomDict.ContainsKey(x)).ToList();
            if (notFoundUomNames.Any())
                return Ok(new { success = false, errors = new List<string>() { $"Đơn vị tính không tồn tại: {string.Join(", ", notFoundUomNames)}" } });

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

            var productsUpdate = new List<Product>();
            var productCodes = data.Select(x => x.DefaultCode).Distinct().ToList();
            var productsToUpdate = await _productService.SearchQuery(x => x.Type2 == "product" && productCodes.Contains(x.DefaultCode))
                .Include(x => x.ProductUoMRels)
                .ToListAsync();
            var productDict = productsToUpdate.GroupBy(x => x.DefaultCode).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var notFoundProductCodes = productCodes.Where(x => !productDict.ContainsKey(x)).ToList();
            if (notFoundProductCodes.Any())
                return Ok(new { success = false, errors = new List<string>() { $"Mã vật tư không tồn tại: {string.Join(", ", notFoundProductCodes)}" } });

            foreach (var item in data)
            {
                if (!productDict.ContainsKey(item.DefaultCode))
                    continue;

                var product = productDict[item.DefaultCode];
                product.Name = item.Name;
                product.NameNoSign = StringUtils.RemoveSignVietnameseV2(item.Name);
                product.CategId = categDict[item.CategName].Id;
                product.PurchasePrice = item.PurchasePrice ?? 0;
                product.UOMId = uomDict[item.UOM].Id;
                product.UOMPOId = uomDict[item.UOMPO].Id;
                product.MinInventory = item.MinInventory ?? 0;
                product.Origin = item.Origin;
                product.Expiry = item.Expiry;
                product.ProductUoMRels.Clear();
                product.ProductUoMRels.Add(new ProductUoMRel { UoMId = uomDict[item.UOM].Id });
                if (uomDict[item.UOM].Id != uomDict[item.UOMPO].Id)
                    product.ProductUoMRels.Add(new ProductUoMRel { UoMId = uomDict[item.UOMPO].Id });

                productsUpdate.Add(product);
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _productService._ComputeUoMRels(productsUpdate);
                await _productService.UpdateAsync(productsUpdate);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                return Ok(new { success = false, errors = new List<string>() { $"Cập nhật thất bại: {e.Message}" } });
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

            _productService._ComputeUoMRels(vals);
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

            _productService._ComputeUoMRels(vals);
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

                worksheet.Cells[1, 1].Value = "Mã dịch vụ";
                worksheet.Cells[1, 2].Value = "Tên dịch vụ";
                worksheet.Cells[1, 3].Value = "Có thể đặt labo";
                worksheet.Cells[1, 4].Value = "Nhóm dịch vụ";
                worksheet.Cells[1, 5].Value = "Giá bán";
                worksheet.Cells[1, 6].Value = "Giá vốn";
                worksheet.Cells[1, 7].Value = "Công đoạn";
                worksheet.Cells[1, 8].Value = "Giá đặt labo";
                worksheet.Cells[1, 9].Value = "Hãng";

                worksheet.Cells["A1:I1"].Style.Font.Bold = true;

                for (int row = 2; row < services.Count() + 2; row++)
                {
                    var item = services.ToList()[row - 2];

                    worksheet.Cells[row, 1].Value = item.DefaultCode;
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.IsLabo;
                    worksheet.Cells[row, 4].Value = item.CategName;
                    worksheet.Cells[row, 5].Value = item.ListPrice;
                    worksheet.Cells[row, 6].Value = item.StandardPrice;
                    worksheet.Cells[row, 7].Value = item.StepList.Count() == 0 ? null : string.Join(";", item.StepList.Select(x => x.Name).ToList());
                    worksheet.Cells[row, 8].Value = item.LaboPrice ?? 0;
                    worksheet.Cells[row, 9].Value = item.Firm;
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
                worksheet.Cells[1, 2].Value = "Mã vật tư";
                worksheet.Cells[1, 3].Value = "Loại";
                worksheet.Cells[1, 4].Value = "Nhóm vật tư";
                worksheet.Cells[1, 5].Value = "Giá mua";
                worksheet.Cells[1, 6].Value = "Đơn vị mặc định";
                worksheet.Cells[1, 7].Value = "Đơn vị mua";
                worksheet.Cells[1, 8].Value = "Mức tồn tối thiểu";
                worksheet.Cells[1, 9].Value = "Xuất xứ";
                worksheet.Cells[1, 10].Value = "Thời hạn sử dụng (tháng)";
                worksheet.Cells["A1:J1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in products)
                {
                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = item.DefaultCode;
                    worksheet.Cells[row, 3].Value = type_dict[item.Type];
                    worksheet.Cells[row, 4].Value = item.CategName;
                    worksheet.Cells[row, 5].Value = item.PurchasePrice ?? 0;
                    worksheet.Cells[row, 6].Value = item.UomName;
                    worksheet.Cells[row, 7].Value = item.UoMPOName;
                    worksheet.Cells[row, 8].Value = item.MinInventory ?? 0;
                    worksheet.Cells[row, 9].Value = item.Origin;
                    worksheet.Cells[row, 10].Value = item.Expiry;
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
                worksheet.Cells[1, 2].Value = "Mã thuốc";
                worksheet.Cells[1, 3].Value = "Nhóm thuốc";
                worksheet.Cells[1, 4].Value = "Giá thuốc";
                worksheet.Cells[1, 5].Value = "Đơn vị tính mặc định";
                worksheet.Cells[1, 6].Value = "Mức tồn tối thiểu";
                worksheet.Cells[1, 7].Value = "Xuất xứ";
                worksheet.Cells[1, 8].Value = "Thời hạn sử dụng (tháng)";
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in products)
                {
                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = item.DefaultCode;
                    worksheet.Cells[row, 3].Value = item.CategName;
                    worksheet.Cells[row, 4].Value = item.ListPrice;
                    worksheet.Cells[row, 5].Value = item.UomName;
                    worksheet.Cells[row, 6].Value = item.MinInventory;
                    worksheet.Cells[row, 7].Value = item.Origin;
                    worksheet.Cells[row, 8].Value = item.Expiry;
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
        public async Task<IActionResult> GenerateServiceXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_service.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample" || x.Module == "product") && (x.Model == "product.category" || (x.Model == "uom"))).ToListAsync();// các irmodel cần thiết
            var entities = await _productService.SearchQuery(x => x.Type2 == "service" && x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<ProductServiceXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<ProductServiceXmlSampleDataRecord>(entity);
                item.Id = $@"sample.product_service_{entities.IndexOf(entity) + 1}";
                var irmodelData = listIrModelData.FirstOrDefault(x => x.ResId == item.CategId.ToString());
                item.CategId = irmodelData?.Module + "." + irmodelData?.Name;
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "product",
                    ResId = entity.Id.ToString(),
                    Name = $"product_service_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateProductXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_product.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample" || x.Module == "product") && (x.Model == "product.category" || (x.Model == "uom"))).ToListAsync();// các irmodel cần thiết
            var entities = await _productService.SearchQuery(x => x.Type2 == "product" && x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<ProductProductXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<ProductProductXmlSampleDataRecord>(entity);
                item.Id = $@"sample.product_product_{entities.IndexOf(entity) + 1}";
                var irmodelData = listIrModelData.FirstOrDefault(x => x.ResId == item.CategId.ToString());
                item.CategId = irmodelData?.Module + "." + irmodelData?.Name;
                var irmodelDataUom = listIrModelData.FirstOrDefault(x => x.ResId == item.UOMId.ToString());
                item.UOMId = irmodelDataUom?.Module + "." + irmodelDataUom?.Name;
                var irmodelDataUomPo = listIrModelData.FirstOrDefault(x => x.ResId == item.UOMPOId.ToString());
                item.UOMPOId = irmodelDataUomPo?.Module + "." + irmodelDataUomPo?.Name;
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "product",
                    ResId = entity.Id.ToString(),
                    Name = $"product_product_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateMedicineXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_medicine.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample" || x.Module == "product") && (x.Model == "product.category" || (x.Model == "uom"))).ToListAsync();// các irmodel cần thiết
            var entities = await _productService.SearchQuery(x => x.Type2 == "medicine" && x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<ProductMedicineXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<ProductMedicineXmlSampleDataRecord>(entity);
                item.Id = $@"sample.product_medicine_{entities.IndexOf(entity) + 1}";
                var irmodelData = listIrModelData.FirstOrDefault(x => x.ResId == item.CategId.ToString());
                item.CategId = irmodelData?.Module + "." + irmodelData?.Name;
                var irmodelDataUom = listIrModelData.FirstOrDefault(x => x.ResId == item.UOMId.ToString());
                item.UOMId = irmodelDataUom?.Module + "." + irmodelDataUom?.Name;
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "product",
                    ResId = entity.Id.ToString(),
                    Name = $"product_medicine_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateLaboXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_labo.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var entities = await _productService.SearchQuery(x => x.Type2 == "labo" && x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<ProductLaboXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<ProductLaboXmlSampleDataRecord>(entity);
                item.Id = $@"sample.product_labo_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "product",
                    ResId = entity.Id.ToString(),
                    Name = $"product_labo_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateLaboAttachXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\product_labo_attach.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var entities = await _productService.SearchQuery(x => x.Type2 == "labo_attach" && x.DateCreated <= dateToData).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<ProductLaboXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<ProductLaboXmlSampleDataRecord>(entity);
                item.Id = $@"sample.product_labo_attach_{entities.IndexOf(entity) + 1}";
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "product",
                    ResId = entity.Id.ToString(),
                    Name = $"product_labo_attach_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
            //string[] listProInfoStr = { "Name","Type","Type2","ListPrice","PurchasePrice","SaleOk","PurchaseOk","KetoanOk","IsLabo","MinInventory",
            //"Firm","CategId", "UOMPOId", "UOMId"};
        }
    }


}