using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResConfigSettingsController : BaseApiController
    {
        private readonly IResConfigSettingsService _configSettingsService;
        private readonly IIRModelDataService _iRModelDataService;
        private readonly IIrModuleCategoryService _iRModuleCategoryService;
        private readonly IIRRuleService _iRuleService;
        private readonly IResGroupService _iResGroupService;
        private readonly IIRModelService _iRModelService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IProductService _productService;

        public ResConfigSettingsController(IResConfigSettingsService configSettingsService, IIRModelDataService iRModelDataService, IIrModuleCategoryService iRModuleCategoryService, IResGroupService iResGroupService,
            IIRRuleService iRuleService, IIRModelService iRModelService, IMapper mapper, IUnitOfWorkAsync unitOfWork, IProductService productService)
        {
            _configSettingsService = configSettingsService;
            _iRModelDataService = iRModelDataService;
            _iResGroupService = iResGroupService;
            _iRuleService = iRuleService;
            _iRModuleCategoryService = iRModuleCategoryService;
            _iRModelService = iRModelService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var config = await _configSettingsService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (config == null)
                return NotFound();
            return Ok(_mapper.Map<ResConfigSettingsDisplay>(config));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResConfigSettingsSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var config = _mapper.Map<ResConfigSettings>(val);
            config.CompanyId = CompanyId;

            if (val.ProductListpriceRestrictCompany == true)
                await _configSettingsService.InsertFieldForProductListPriceRestrictCompanies();

            await _configSettingsService.CreateAsync(config);

            var basic = _mapper.Map<ResConfigSettingsBasic>(config);
            return Ok(basic);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> Excute(Guid id)
        {
            var config = await _configSettingsService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (config == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _configSettingsService.Excute(config);

            if (config.GroupMedicine.HasValue && config.GroupMedicine.Value)
            {
                var purchaseOK = config.GroupMedicine == true;
                var medicines = await _productService.SearchQuery(x => x.Type2 == "medicine").ToListAsync();
                foreach (var medicine in medicines)
                    medicine.PurchaseOK = purchaseOK;
                await _productService.UpdateAsync(medicines);
            }

            if (config.GroupSurvey.HasValue && config.GroupSurvey.Value)
            {
                //nếu chưa có ir module category thi tạo và insert các groups add ir model data
                var categ = await _iRModelDataService.GetRef<IrModuleCategory>("survey.module_category_survey");
                if (categ == null)
                {
                    ///create irmodulecategory
                    categ = await _iRModuleCategoryService.CreateAsync(new IrModuleCategory()
                    {
                        Name = "Survey",
                        Visible = false
                    });

                    var surveyAssignmentModel = await _iRModelService.CreateAsync(new IRModel()
                    {
                        Name = "Survey Assignment",
                        Model = "SurveyAssignment",
                        Transient = true,
                    });

                    var surveyAssignmentRule = await _iRuleService.CreateAsync(new IRRule()
                    {
                        Name = "survey assignment by employee",
                        ModelId = surveyAssignmentModel.Id,
                        Code = "survey.assignment_employee",
                    });

                    /// create resgroups
                    var empGroup = new ResGroup()
                    {
                        CategoryId = categ.Id,
                        Name = "Nhân viên",
                    };
                    empGroup.RuleGroupRels.Add(new RuleGroupRel { RuleId = surveyAssignmentRule.Id });

                    var manageGroup = new ResGroup()
                    {
                        CategoryId = categ.Id,
                        Name = "Quản lý",
                    };

                    var groups = await _iResGroupService.CreateAsync(new List<ResGroup>() { manageGroup, empGroup });

                    //tao model data quản lý 2 group
                    var modelData = await _iRModelDataService.CreateAsync(new List<IRModelData>() {
                        new IRModelData() // for show combobox
                        {
                             Name = "module_category_survey",
                             Module = "survey",
                             ResId = categ.Id.ToString(),
                             Model = "ir.module.category"
                        },
                        new IRModelData()
                        {
                            Name = "survey_assignment_manager",
                            Module = "survey",
                            ResId = manageGroup.Id.ToString(),
                            Model = "res.groups"
                        },
                        new IRModelData()
                        {
                            Name = "survey_assignment_user",
                            Module = "survey",
                            ResId = empGroup.Id.ToString(),
                            Model = "res.groups"
                        }
                    });
                }
            }

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _configSettingsService.DefaultGet<ResConfigSettingsDisplay>();
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> InsertServiceCardData()
        {
            await _unitOfWork.BeginTransactionAsync();
            await _configSettingsService.InsertServiceCardData();
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}