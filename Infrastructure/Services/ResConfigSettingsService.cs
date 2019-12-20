using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ResConfigSettingsService: BaseService<ResConfigSettings>, IResConfigSettingsService
    {
        public ResConfigSettingsService(IAsyncRepository<ResConfigSettings> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public virtual async Task<T> DefaultGet<T>()
        {
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

            var groupObj = GetService<IResGroupService>();
            //await groupObj.InsertGroupUserIfNotExist();
            //await groupObj.InsertSettingGroupIfNotExist("sale.group_discount_per_so_line", "Discount on lines");
            //await groupObj.InsertSettingGroupIfNotExist("sale.group_sale_coupon_promotion", "Coupon Promotion Programs");
            //await groupObj.InsertSettingGroupIfNotExist("sale.group_loyalty_card", "Loyalty Card");

            //var user = UserSessionCtx.User;
            //if (!user.HasGroup("base.group_system"))
            //    throw new Exception("Chỉ có admin mới được thay đổi thiết lập");
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

                implied_group.ResGroupsUsersRels.Clear();
                await groupObj.UpdateAsync(implied_group);
            }

            var irConfigParameter = GetService<IIrConfigParameterService>();
            foreach(var item in classified.Configs)
            {
                var field_type = item.FieldType;
                var value = self.GetType().GetProperty(item.Name).GetValue(self, null);
                var valueStr = "";
                if (field_type == "decimal")
                    valueStr = Convert.ToDecimal(value).ToString();
                await irConfigParameter.SetParam(item.ConfigParameter, valueStr);
            }

            //other fields
            await SetDefaultOtherFields(self, classified.Others);
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
