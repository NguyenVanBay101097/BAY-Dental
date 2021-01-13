using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ResConfigSettingsService: BaseService<ResConfigSettings>, IResConfigSettingsService
    {
        private readonly ITCareJobService _tcareJobService;
        private readonly AppTenant _tenant;
        private readonly IMyCache _cache;

        public ResConfigSettingsService(IAsyncRepository<ResConfigSettings> repository, IHttpContextAccessor httpContextAccessor,
            ITCareJobService tcareJobService, ITenant<AppTenant> tenant, IMyCache cache)
            : base(repository, httpContextAccessor)
        {
            _tcareJobService = tcareJobService;
            _tenant = tenant?.Value;
            _cache = cache;
        }

        public virtual async Task<T> DefaultGet<T>()
        {
            var groupObj = GetService<IResGroupService>();
            await groupObj.InsertSettingGroupIfNotExist("product.group_uom", "Group UoM");
            await groupObj.InsertSettingGroupIfNotExist("medicineOrder.group_medicine", "Group Medicine");
            await groupObj.InsertSettingGroupIfNotExist("sale.group_service_card", "Service Card");
            await groupObj.InsertSettingGroupIfNotExist("tcare.group_tcare", "TCare");

            //var irValueObj = DependencyResolver.Current.GetService<IRValuesService>();
            var classified = await _GetClassifiedFields<T>();
            var res = Activator.CreateInstance<T>();
            //defaults: take the corresponding default value they set
            //foreach (var item in classified.Defaults)
            //{
            //    var name = item.Name;
            //    var model = item.Model;
            //    var field = item.Field;
            //    var field_type = item.FieldType;

            //    var value = irValueObj.GetDefault(model, field, field_type);
            //    if (value != null)
            //        res.GetType().GetProperty(name).SetValue(res, value);
            //}

            foreach (var item in classified.Groups)
            {
                var name = item.Name;
                var field_type = item.FieldType;
                var groups = item.FieldGroups;
                var implied_group = item.ImpliedGroup;
                var value = groups.All(x => x.ImpliedRels.Any(s => s.HId == implied_group.Id));
                if (field_type == "integer")
                    res.GetType().GetProperty(name).SetValue(res, Convert.ToInt32(value));
                else
                    res.GetType().GetProperty(name).SetValue(res, value);
            }

            var irConfigParameter = GetService<IIrConfigParameterService>();
            foreach (var item in classified.Configs)
            {
                var name = item.Name;
                var field_type = item.FieldType;
                var value = await irConfigParameter.GetParam(item.ConfigParameter);
                if (!string.IsNullOrEmpty(value))
                {
                    if (field_type == "decimal")
                        res.GetType().GetProperty(name).SetValue(res, Convert.ToDecimal(value));
                    else if (field_type == "boolean")
                        res.GetType().GetProperty(name).SetValue(res, Convert.ToBoolean(value));
                    else if (field_type == "datetime")
                        res.GetType().GetProperty(name).SetValue(res, Convert.ToDateTime(value));
                }
            }

            await GetDefaultOtherFields(res, classified.Others);
            return res;
        }

        private async Task GetDefaultOtherFields<T>(T self, IList<string> fields)
        {
            var modelDataObj = GetService<IIRModelDataService>();
            foreach (var field in fields)
            {
                if (field == "CompanyShareProduct")
                {
                    var value = Convert.ToBoolean(self.GetType().GetProperty(field).GetValue(self, null));
                    var productRule = await modelDataObj.GetRef<IRRule>("product.product_comp_rule");
                    self.GetType().GetProperty(field).SetValue(self, !productRule.Active);
                }

                if (field == "CompanySharePartner")
                {
                    var value = Convert.ToBoolean(self.GetType().GetProperty(field).GetValue(self, null));
                    var partnerRule = await modelDataObj.GetRef<IRRule>("base.res_partner_rule");
                    self.GetType().GetProperty(field).SetValue(self, !partnerRule.Active);
                }
            }
        }

        public virtual async Task Excute<T>(T self)
        {
            var userObj = GetService<IUserService>();
            if (!await userObj.HasGroup("base.group_system"))
                throw new Exception("Chỉ có admin mới được thay đổi thiết lập");

            //Đảm bảo tài khoản admin sẽ kế thừa group base.group_user
            await _EnsureAdminHasGroupUser();

            var groupObj = GetService<IResGroupService>();

            var classified = await _GetClassifiedFields<T>();

            //foreach (var item in classified.Defaults)
            //{
            //    var name = item.Name;
            //    var model = item.Model;
            //    var field = item.Field;
            //    var field_type = item.FieldType;

            //    var value = self.GetType().GetProperty(name).GetValue(self, null);
            //    irValueObj.SetDefault(model, field, field_type, value);
            //}

            foreach (var item in classified.Groups)
            {
                var name = item.Name;
                var groups = item.FieldGroups;
                var implied_group = item.ImpliedGroup;
                if (implied_group == null)
                    continue;
                var val = self.GetType().GetProperty(name).GetValue(self, null);
                implied_group.ResGroupsUsersRels.Clear();
                await groupObj.UpdateAsync(implied_group);
                if (Equals(val, 1) || Equals(val, true))
                {
                    foreach (var group in groups)
                    {
                        if (!group.ImpliedRels.Any(x => x.HId == implied_group.Id))
                            group.ImpliedRels.Add(new ResGroupImpliedRel { HId = implied_group.Id });
                    }

                    await groupObj.UpdateAsync(groups);
                    await groupObj.AddAllImpliedGroupsToAllUser(groups);
                    //groupObj.Write(groups, implied_ids: new List<ResGroup>() { implied_group });

                    //if (name == "GroupTCare")
                    //{
                    //    var db = _tenant != null ? _tenant.Hostname : "localhost";
                    //    var runAtConfig = classified.Configs.FirstOrDefault(x => x.Name == "TCareRunAt");
                    //    var runAt = Convert.ToDateTime(self.GetType().GetProperty(runAtConfig.Name).GetValue(self, null));
                    //    RecurringJob.AddOrUpdate($"tcare-{db}", () => _tcareJobService.Run(db), $"{runAt.Minute} {runAt.Hour} * * *", TimeZoneInfo.Local);
                    //}
                }
                else
                {
                    foreach (var group in groups)
                    {
                        var rel = group.ImpliedRels.FirstOrDefault(x => x.HId == implied_group.Id);
                        if (rel != null)
                            group.ImpliedRels.Remove(rel);
                    }

                    await groupObj.UpdateAsync(groups);
                    await groupObj.AddAllImpliedGroupsToAllUser(groups);

                    //groupObj.Write(groups, implied_ids: new List<ResGroup>() { implied_group });
                    //var users = groups.SelectMany(x => x.Users);
                    //foreach (var user in users)
                    //{
                    //    implied_group.Users.Remove(user);
                    //}
                    //groupObj.Write(new List<ResGroup>() { implied_group }, users: users);
                }
            }

            var irConfigParameter = GetService<IIrConfigParameterService>();
            foreach(var item in classified.Configs)
            {
                var field_type = item.FieldType;
                var value = self.GetType().GetProperty(item.Name).GetValue(self, null);
                var valueStr = "";
                if (field_type == "decimal")
                    valueStr = Convert.ToDecimal(value).ToString();
                else if (field_type == "boolean")
                    valueStr = Convert.ToBoolean(value).ToString();
                else if (field_type == "datetime")
                    valueStr = Convert.ToDateTime(value).ToString();
                await irConfigParameter.SetParam(item.ConfigParameter, valueStr);
            }

            //other fields
            await SetDefaultOtherFields(self, classified.Others);

            //xử lý clear cache
            _cache.RemoveByPattern($"{(_tenant != null ? _tenant.Hostname : "localhost")}-ir.rule");
        }

        private async Task _EnsureAdminHasGroupUser()
        {
            var userManager = GetService<UserManager<ApplicationUser>>();
            var userRoot = userManager.Users.Where(x => x.IsUserRoot).Include(x => x.ResGroupsUsersRels).FirstOrDefault();
            if (userRoot != null)
            {
                var modelDataObj = GetService<IIRModelDataService>();
                var groupUser = await modelDataObj.GetRef<ResGroup>("base.group_user");
                if (!userRoot.ResGroupsUsersRels.Any(x => x.GroupId == groupUser.Id))
                {
                    userRoot.ResGroupsUsersRels.Add(new ResGroupsUsersRel { GroupId = groupUser.Id });
                    var updateResult = await userManager.UpdateAsync(userRoot);
                    if (!updateResult.Succeeded)
                        throw new Exception(string.Join(", ", updateResult.Errors.Select(x => x.Description)));
                }
            }
        }

        public async Task InsertServiceCardData()
        {
            //insert models
            var modelObj = GetService<IIRModelService>();
            var model_dict = new Dictionary<string, IRModel>()
            {
                {"service_card.model_service_card_card", new IRModel { Name = "Thẻ dịch vụ", Model = "ServiceCardCard", Transient = false }},
                {"service_card.model_service_card_order", new IRModel { Name = "Đơn bán thẻ dịch vụ", Model = "ServiceCardOrder", Transient = false }},
                {"service_card.model_service_card_type", new IRModel { Name = "Loại thẻ dịch vụ", Model = "ServiceCardType", Transient = false }},
                {"service_card.model_service_card_sale_order_service_card_card_rel", new IRModel { Name = "Sale Order Card Rel", Model = "SaleOrderServiceCardCardRel", Transient = true }},
            };

            var new_model_dict = await modelObj.InsertIfNotExist(model_dict);

            //insert groups
            var groupObj = GetService<IResGroupService>();
            var group_dict = new Dictionary<string, ResGroup>()
            {
                {"sale.group_user", new ResGroup { Name = "Lễ tân" }},
                {"sale.group_manager", new ResGroup { Name = "Bác sĩ" }},
                {"sale.group_admin", new ResGroup { Name = "Chủ chi nhánh" }},
            };

            var new_group_dict = await groupObj.InsertIfNotExist(group_dict);

            //insert ir model accesses
            var accessObj = GetService<IIRModelAccessService>();
            var access_dict = new Dictionary<string, IRModelAccess>()
            {
                {"service_card.access_service_card_card_group_user", new IRModelAccess { Name = "service_card_card group_user", ModelId = new_model_dict["service_card.model_service_card_card"].Id, Group = new_group_dict["sale.group_user"], PermRead = true, PermWrite = true, PermCreate = true, PermUnlink = true  }},
                {"service_card.access_service_card_type_group_user", new IRModelAccess { Name = "service_card_type group_user", ModelId = new_model_dict["service_card.model_service_card_type"].Id, Group = new_group_dict["sale.group_user"], PermRead = true, PermWrite = true, PermCreate = true, PermUnlink = true }},
                {"service_card.access_service_card_order_group_user", new IRModelAccess { Name = "service_card_order group_user", ModelId = new_model_dict["service_card.model_service_card_order"].Id, Group = new_group_dict["sale.group_user"], PermRead = true, PermWrite = true, PermCreate = true, PermUnlink = true }},
                {"service_card.access_service_card_card_group_manager", new IRModelAccess { Name = "service_card_card group_manager", ModelId = new_model_dict["service_card.model_service_card_card"].Id, Group = new_group_dict["sale.group_manager"], PermRead = true, PermWrite = false, PermCreate = false, PermUnlink = false  }},
                {"service_card.access_service_card_type_group_manager", new IRModelAccess { Name = "service_card_type group_manager", ModelId = new_model_dict["service_card.model_service_card_type"].Id, Group = new_group_dict["sale.group_manager"], PermRead = true, PermWrite = false, PermCreate = false, PermUnlink = false }},
                {"service_card.access_service_card_order_group_manager", new IRModelAccess { Name = "service_card_order group_manager", ModelId = new_model_dict["service_card.model_service_card_order"].Id, Group = new_group_dict["sale.group_manager"], PermRead = true, PermWrite = false, PermCreate = false, PermUnlink = false }},
                {"service_card.access_service_card_card_group_admin", new IRModelAccess { Name = "service_card_card group_admin", ModelId = new_model_dict["service_card.model_service_card_card"].Id, Group = new_group_dict["sale.group_admin"], PermRead = true, PermWrite = false, PermCreate = false, PermUnlink = false  }},
                {"service_card.access_service_card_type_group_admin", new IRModelAccess { Name = "service_card_type group_admin", ModelId = new_model_dict["service_card.model_service_card_type"].Id, Group = new_group_dict["sale.group_admin"], PermRead = true, PermWrite = false, PermCreate = false, PermUnlink = false }},
                {"service_card.access_service_card_order_group_admin", new IRModelAccess { Name = "service_card_order group_admin", ModelId = new_model_dict["service_card.model_service_card_order"].Id, Group = new_group_dict["sale.group_admin"], PermRead = true, PermWrite = false, PermCreate = false, PermUnlink = false }},
            };

            await accessObj.InsertIfNotExist(access_dict);

            await groupObj.InsertSettingGroupIfNotExist("sale.group_service_card", "Service Card");

            //insert rules
            var ruleObj = GetService<IIRRuleService>();
            var rule_dict = new Dictionary<string, IRRule>()
            {
                {"service_card.service_card_order_comp_rule", new IRRule { Name = "Service Card Order multi-company", ModelId = new_model_dict["service_card.model_service_card_order"].Id }},
                {"service_card.service_card_type_comp_rule", new IRRule { Name = "Service Card Type multi-company", ModelId = new_model_dict["service_card.model_service_card_type"].Id }},
                {"service_card.service_card_card_comp_rule", new IRRule { Name = "Service Card Card multi-company", ModelId = new_model_dict["service_card.model_service_card_card"].Id }},
            };
            await ruleObj.InsertIfNotExist(rule_dict);
        }

        public async Task InsertFieldForProductListPriceRestrictCompanies()
        {
            var fieldObj = GetService<IIRModelFieldService>();
            var field = await fieldObj.SearchQuery(x => x.Name == "list_price" && x.Model == "product.product").FirstOrDefaultAsync();
            if (field == null)
            {
                var modelObj = GetService<IIRModelService>();
                var model = await modelObj.SearchQuery(x => x.Model == "Product").FirstOrDefaultAsync();
                field = new IRModelField
                {
                    IRModelId = model.Id,
                    Model = "product.product",
                    Name = "list_price",
                    TType = "decimal",
                };

                await fieldObj.CreateAsync(field);
            }
        }

        public async Task SetDefaultOtherFields<T>(T self, IList<string> fields)
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var ruleObj = GetService<IIRRuleService>();
            foreach (var field in fields)
            {
                if (field == "CompanyShareProduct")
                {
                    var value = Convert.ToBoolean(self.GetType().GetProperty(field).GetValue(self, null));
                    var productRule = await modelDataObj.GetRef<IRRule>("product.product_comp_rule");
                    productRule.Active = !value;
                    await ruleObj.UpdateAsync(productRule);
                }

                if (field == "CompanySharePartner")
                {
                    var value = Convert.ToBoolean(self.GetType().GetProperty(field).GetValue(self, null));
                    var partnerRule = await modelDataObj.GetRef<IRRule>("base.res_partner_rule");
                    partnerRule.Active = !value;
                    await ruleObj.UpdateAsync(partnerRule);
                }
            }
        }

        private async Task<GetClassifiedFieldsRes> _GetClassifiedFields<T>()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var groupObj = GetService<IResGroupService>();
            var groups = new List<GetClassifiedFieldsGroup>();
            var defaults = new List<GetClassifiedFieldsDefault>();
            var configs = new List<GetClassifiedFieldsConfig>();
            var others = new List<string>();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var name = prop.Name;
                var attrs = (DbColumnAttribute[])prop.GetCustomAttributes(typeof(DbColumnAttribute), false);
                var field_type = attrs.FirstOrDefault(x => x.Name == "field_type") != null ? attrs.FirstOrDefault(x => x.Name == "field_type").Value : prop.PropertyType.Name;

                if (name.StartsWith("Default") && attrs.Any(x => x.Name == "default_model") && attrs.Any(x => x.Name == "default_name"))
                {
                    defaults.Add(new GetClassifiedFieldsDefault
                    {
                        Name = name,
                        Model = attrs.First(x => x.Name == "default_model").Value,
                        Field = attrs.First(x => x.Name == "default_name").Value,
                        FieldType = field_type,
                    });
                }
                else if (name.StartsWith("Group") && attrs.Any(x => x.Name == "implied_group"))
                {
                    var group_attr = attrs.FirstOrDefault(x => x.Name == "group");
                    var implied_group_attr = attrs.First(x => x.Name == "implied_group");
                    var field_group_xmlids = (group_attr != null ? group_attr.Value : "base.group_user").Split(',');
                    var field_groups = new List<ResGroup>();
                    foreach (var field_group_xmlid in field_group_xmlids)
                    {
                        var group = await irModelDataObj.GetRef<ResGroup>(field_group_xmlid);
                        if (group != null)
                            field_groups.Add(await groupObj.SearchQuery(x => x.Id == group.Id).Include(x => x.ImpliedRels).FirstOrDefaultAsync()); //find by id because cache
                    }

                    var implied_group = await irModelDataObj.GetRef<ResGroup>(implied_group_attr.Value);
                    groups.Add(new GetClassifiedFieldsGroup
                    {
                        FieldGroups = field_groups,
                        ImpliedGroup = implied_group != null ? (await groupObj.SearchQuery(x => x.Id == implied_group.Id).Include(x => x.ResGroupsUsersRels).FirstOrDefaultAsync()) : null,
                        Name = name,
                        FieldType = field_type,
                    });
                }
                else if (attrs.Any(x => x.Name == "config_parameter"))
                {
                    var config_parameter = attrs.FirstOrDefault(x => x.Name == "config_parameter");
                    configs.Add(new GetClassifiedFieldsConfig { Name = name, ConfigParameter = config_parameter.Value, FieldType = field_type });
                }
                else
                {
                    others.Add(name);
                }
            }
            return new GetClassifiedFieldsRes
            {
                Groups = groups,
                Defaults = defaults,
                Configs = configs,
                Others = others
            };
        }

        public class GetClassifiedFieldsRes
        {
            public IList<GetClassifiedFieldsGroup> Groups { get; set; }

            public IList<GetClassifiedFieldsDefault> Defaults { get; set; }

            public IList<GetClassifiedFieldsConfig> Configs { get; set; }

            public IList<string> Others { get; set; }
        }

        public class GetClassifiedFieldsGroup
        {
            public string Name { get; set; }

            public IEnumerable<ResGroup> FieldGroups { get; set; }

            public ResGroup ImpliedGroup { get; set; }

            public string FieldType { get; set; }
        }

        public class GetClassifiedFieldsConfig
        {
            public string Name { get; set; }
            public string ConfigParameter { get; set; }
            public string FieldType { get; set; }
        }

        public class GetClassifiedFieldsDefault
        {
            public string Name { get; set; }

            public string Model { get; set; }

            public string Field { get; set; }

            public string FieldType { get; set; }
        }
    }
}
