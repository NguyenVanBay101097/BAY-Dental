using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ResGroupService : BaseService<ResGroup>, IResGroupService
    {
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMyCache _cache;

        public ResGroupService(IAsyncRepository<ResGroup> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IHostingEnvironment hostingEnvironment, IMyCache cache)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        private IQueryable<ResGroup> GetQueryPaged(ResGroupPaged val)
        {
            var query = SearchQuery(x => !x.CategoryId.HasValue || x.Category.Visible == true);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            query = query.OrderBy(s => s.Name);
            return query;
        }

        public async Task<PagedResult2<ResGroupBasic>> GetPagedResultAsync(ResGroupPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<ResGroupBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResGroupBasic>>(items)
            };
        }

        public async Task<ResGroupDisplay> DefaultGet()
        {
            var res = new ResGroupDisplay();
            var modelObj = GetService<IIRModelService>();
            var models = await modelObj.SearchQuery(x => !x.Transient, orderBy: x => x.OrderBy(s => s.Name)).ToListAsync();
            var list = new List<IRModelAccessDisplay>();
            foreach(var model in models)
            {
                list.Add(new IRModelAccessDisplay
                {
                    Name = model.Name,
                    ModelId = model.Id,
                    Model = _mapper.Map<IRModelBasic>(model)
                });
            }
            res.ModelAccesses = list;
            return res;
        }

        public override Task<ResGroup> CreateAsync(ResGroup entity)
        {
            return base.CreateAsync(entity);
        }

        //function to add all implied groups (to all users of each group)
        public async Task AddAllImpliedGroupsToAllUser(IEnumerable<ResGroup> self)
        {
            foreach(var group in self)
            {
                await ExcuteSqlCommandAsync(" WITH group_imply(gid, hid) AS ( " +
                        "SELECT GId as gid, HId as hid " +
                          "FROM ResGroupImpliedRels " +
                         "UNION ALL " +
                        "SELECT i.gid, r.HId as hid " +
                          "FROM ResGroupImpliedRels r " +
                          "JOIN group_imply i ON(i.hid = r.GId) " +
                    ") " +
                    "INSERT INTO ResGroupsUsersRels(GroupId, UserId) " +
                         "SELECT i.hid, r.UserId " +
                           "FROM group_imply i, ResGroupsUsersRels r " +
                          "WHERE r.GroupId = i.gid " +
                            "AND i.gid = @p0 " +
                         "EXCEPT " +
                         "SELECT r.GroupId, r.UserId " +
                           "FROM ResGroupsUsersRels r " +
                           "JOIN group_imply i ON(r.GroupId = i.hid) " +
                          "WHERE i.gid = @p0", group.Id);
            }
        }

        public async Task AddAllImpliedGroupsToAllUser(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                await ExcuteSqlCommandAsync(" WITH group_imply(gid, hid) AS ( " +
                        "SELECT GId as gid, HId as hid " +
                          "FROM ResGroupImpliedRels " +
                         "UNION ALL " +
                        "SELECT i.gid, r.HId as hid " +
                          "FROM ResGroupImpliedRels r " +
                          "JOIN group_imply i ON(i.hid = r.GId) " +
                    ") " +
                    "INSERT INTO ResGroupsUsersRels(GroupId, UserId) " +
                         "SELECT i.hid, r.UserId " +
                           "FROM group_imply i, ResGroupsUsersRels r " +
                          "WHERE r.GroupId = i.gid " +
                            "AND i.gid = @p0 " +
                         "EXCEPT " +
                         "SELECT r.GroupId, r.UserId " +
                           "FROM ResGroupsUsersRels r " +
                           "JOIN group_imply i ON(r.GroupId = i.hid) " +
                          "WHERE i.gid = @p0", id);
            }
        }

        public void Write(IEnumerable<ResGroup> groups, List<ResGroup> implied_ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ResGroup> InsertGroupUserIfNotExist()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var group = await modelDataObj.GetRef<ResGroup>("base.group_user");
            if (group == null)
            {
                var categObj = GetService<IIrModuleCategoryService>();
                var category = await modelDataObj.GetRef<IrModuleCategory>("base.module_category_user_type");
                if (category == null)
                {
                    category = new IrModuleCategory
                    {
                        Name = "User types",
                        Description = "Helps you manage users.",
                        Sequence = 9,
                        Visible = false,
                        Exclusive = false,
                    };
                    await categObj.CreateAsync(category);

                    var categModelData = new IRModelData()
                    {
                        Model = "ir.module.category",
                        Name = "module_category_user_type",
                        Module = "base",
                        ResId = category.Id.ToString(),
                    };

                    await modelDataObj.CreateAsync(categModelData);
                }
              
                group = new ResGroup {
                    Name = "Internal User",
                    CategoryId = category.Id
                };
                await CreateAsync(group);

                var modelData = new IRModelData()
                {
                    Model = "res.groups",
                    Name = "group_user",
                    Module = "base",
                    ResId = group.Id.ToString(),
                };
                await modelDataObj.CreateAsync(modelData);

                await UpdateAllGroupImpliedGroupUser(group);
            }

            return group;
        }

        public async Task<IDictionary<string, ResGroup>> InsertIfNotExist(IDictionary<string, ResGroup> dict)
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var new_dict = new Dictionary<string, ResGroup>();
            foreach (var item in dict)
            {
                var reference = item.Key;
                var model = await modelDataObj.GetRef<ResGroup>(reference);
                if (model == null)
                {
                    model = item.Value;
                    await CreateAsync(model);

                    var referenceSplit = reference.Split(".");

                    var modelData = new IRModelData()
                    {
                        Model = "res.groups",
                        Name = referenceSplit[1],
                        Module = referenceSplit[0],
                        ResId = model.Id.ToString(),
                    };
                    await modelDataObj.CreateAsync(modelData);
                }

                new_dict.Add(reference, model);
            }

            return new_dict;
        }


        public async Task UpdateAllGroupImpliedGroupUser(ResGroup groupUser)
        {
            var groups = await SearchQuery(x => x.Id != groupUser.Id && (x.Category == null || x.Category.Visible == false)).Include(x => x.ImpliedRels).ToListAsync();
            foreach(var group in groups)
            {
                if (!group.ImpliedRels.Any(x => x.HId == groupUser.Id))
                    group.ImpliedRels.Add(new ResGroupImpliedRel { HId = groupUser.Id });
            }
            await UpdateAsync(groups);
            await AddAllImpliedGroupsToAllUser(groups);
        }

        public async Task<ResGroup> InsertSettingGroupIfNotExist(string reference, string name)
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var group = await modelDataObj.GetRef<ResGroup>(reference);
            if (group == null)
            {
                var categObj = GetService<IIrModuleCategoryService>();
                var categ = await modelDataObj.GetRef<IrModuleCategory>("base.module_category_hidden");
                if (categ == null)
                {
                    categ = new IrModuleCategory()
                    {
                        Name = "Technical Settings",
                        Sequence = 0,
                        Visible = false,
                    };
                    await categObj.CreateAsync(categ);

                    var categModelData = new IRModelData()
                    {
                        Model = "ir.module.category",
                        Name = "module_category_hidden",
                        Module = "base",
                        ResId = categ.Id.ToString(),
                    };

                    await modelDataObj.CreateAsync(categModelData);
                }

                group = new ResGroup
                {
                    Name = name,
                    CategoryId = categ.Id
                };
                await CreateAsync(group);

                var referenceSplit = reference.Split(".");
               
                var modelData = new IRModelData()
                {
                    Model = "res.groups",
                    Name = referenceSplit[1],
                    Module = referenceSplit[0],
                    ResId = group.Id.ToString(),
                };
                await modelDataObj.CreateAsync(modelData);
            }

            return group;
        }

        public async Task ResetSecurityData()
        {
            //xóa tất cả dữ liệu
            await ExcuteSqlCommandAsync("delete IRModelAccesses");
            await ExcuteSqlCommandAsync("delete IRRules");
            await ExcuteSqlCommandAsync("delete IRModels");
            await ExcuteSqlCommandAsync("delete ResGroupImpliedRels");
            await ExcuteSqlCommandAsync("delete ResGroupsUsersRels");
            await ExcuteSqlCommandAsync("delete ResGroups");
            await ExcuteSqlCommandAsync("delete IrModuleCategories");
            await ExcuteSqlCommandAsync("delete IRModelDatas where Model = 'res.groups' or Model = 'ir.module.category' or Model='ir.rule' or Model='ir.model'");

            //insert lại dữ liệu
            await InsertSecurityData();

            //clear cache
            ClearCache();
        }

        public void ClearCache()
        {
            _cache.RemoveByPattern("ir.model.access");
            _cache.RemoveByPattern("ir.rule");
        }

        public async Task InsertSecurityData()
        {
            var moduleCategObj = GetService<IIrModuleCategoryService>();
            var modelObj = GetService<IIRModelService>();
            var ruleObj = GetService<IIRRuleService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var userManagerObj = GetService<UserManager<ApplicationUser>>();

            var modelDataList = new List<IRModelData>();

            var base_user_dict = new Dictionary<string, ApplicationUser>();
            var user_root = await modelDataObj.GetRef<ApplicationUser>("base.user_root");
            if (user_root == null)
            {
                user_root = await userManagerObj.Users.Where(x => x.IsUserRoot == true).FirstOrDefaultAsync();
                if (user_root != null)
                {
                    base_user_dict.Add("base.user_root", user_root);
                    modelDataList.AddRange(GetModelData(base_user_dict, "res.users"));
                }
            }
            else
                base_user_dict.Add("base.user_root", user_root);

            if (user_root == null)
                throw new Exception("Không tìm thấy user root");

            //read danh sách ir module category đưa vào dictionary
            var moduleCategDict = GetModuleCategoryDict();
            await moduleCategObj.CreateAsync(moduleCategDict.Values);

            var modelDict = GetModelDict();
            await modelObj.CreateAsync(modelDict.Values);

            modelDataList.AddRange(GetModelData(modelDict, "ir.model"));

            var groupDict = new Dictionary<string, ResGroup>();
            var ruleDict = new Dictionary<string, IRRule>();
            var accessDict = new Dictionary<string, IRModelAccess>();

           
            modelDataList.AddRange(GetModelData(moduleCategDict, "ir.module.category"));

            void GetGroupRuleDict(string fileName, string module = "base")
            {
                var xml_file_path = Path.Combine(_hostingEnvironment.ContentRootPath, $@"SampleData\{fileName}");
                if (!File.Exists(xml_file_path))
                    return;
                XmlDocument doc = new XmlDocument();
                doc.Load(xml_file_path);
                var records = doc.GetElementsByTagName("record");
                for (var i = 0; i < records.Count; i++)
                {
                    XmlElement record = (XmlElement)records[i];
                    var model = record.GetAttribute("model");
                    var id = record.GetAttribute("id").IndexOf(".") == -1 ? module + "." + record.GetAttribute("id") : record.GetAttribute("id");
                    if (model == "res.groups")
                    {
                        var mdl = new ResGroup();
                        var fields = record.GetElementsByTagName("field");
                        for (var j = 0; j < fields.Count; j++)
                        {
                            XmlElement field = (XmlElement)fields[j];
                            var field_name = field.GetAttribute("name");
                            if (field_name == "name")
                            {
                                mdl.Name = field.InnerText;
                            }
                            else if (field_name == "implied_ids")
                            {
                                var vals = field.GetAttribute("eval").Split(',').Select(x => x.IndexOf(".") == -1 ? module + "." + x : x);
                                if (vals.Any(x => !groupDict.ContainsKey(x)))
                                    throw new Exception($"Không tìm thấy groups với id: {string.Join(", ", vals)}");

                                var implieds = vals.Select(x => groupDict[x]).ToList();
                                foreach (var implied in implieds)
                                    mdl.ImpliedRels.Add(new ResGroupImpliedRel { H = implied });
                            }
                            else if (field_name == "users")
                            {
                                var vals = field.GetAttribute("eval").Split(',').Select(x => x.IndexOf(".") == -1 ? module + "." + x : x);
                                if (vals.Any(x => !base_user_dict.ContainsKey(x)))
                                    throw new Exception($"Không tìm thấy users với id: {string.Join(", ", vals)}");

                                var usrs = vals.Select(x => base_user_dict[x]).ToList();
                                foreach(var usr in usrs)
                                    mdl.ResGroupsUsersRels.Add(new ResGroupsUsersRel { UserId = usr.Id });
                            }
                            else if (field_name == "category_id")
                            {
                                var vals = field.GetAttribute("ref");
                                vals = vals.IndexOf(".") == -1 ? module + "." + vals : vals;
                                if (!moduleCategDict.ContainsKey(vals))
                                    throw new Exception("module not found: " + vals);
                                mdl.CategoryId = moduleCategDict[vals].Id;
                            }
                        }
                        groupDict.Add(id, mdl);
                    }
                    else if (model == "ir.rule")
                    {
                        var rule = new IRRule();
                        rule.Code = id;
                        var fields = record.GetElementsByTagName("field");
                        for (var j = 0; j < fields.Count; j++)
                        {
                            XmlElement field = (XmlElement)fields[j];
                            var field_name = field.GetAttribute("name");
                            if (field_name == "name")
                            {
                                rule.Name = field.InnerText;
                            }
                            else if (field_name == "model_id")
                            {
                                var vals = field.GetAttribute("ref");
                                vals = vals.IndexOf(".") == -1 ? module + "." + vals : vals;
                                rule.Model = modelDict[vals];
                            }
                            else if (field_name == "global")
                            {
                                var vals = field.GetAttribute("eval");
                                rule.Global = Boolean.Parse(vals);
                            }
                        }
                        ruleDict.Add(id, rule);
                    }
                }
            }

            GetGroupRuleDict("base_groups.xml", module: "base");

            GetGroupRuleDict("dental_security.xml", module: "sale");

            await CreateAsync(groupDict.Values);
            await ruleObj.CreateAsync(ruleDict.Values);

            async Task InsertAccesses(string module = "base")
            {
                var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ir_model_access.csv");
                if (!File.Exists(file_path))
                    return;
                var accessObj = GetService<IIRModelAccessService>();
                using (TextReader reader = File.OpenText(file_path))
                {
                    var csv = new CsvReader(reader);
                    var records = csv.GetRecords<IRModelAccessCsvLine>().ToList();

                    var errors = new List<string>();
                    foreach (var record in records)
                    {
                        var errs = new List<string>();
                        var model_id = record.model_id.IndexOf(".") == -1 ? module + "." + record.model_id : record.model_id;
                        var group_id = record.group_id.IndexOf(".") == -1 ? module + "." + record.group_id : record.group_id;

                        accessDict.Add(record.id, new IRModelAccess
                        {
                            Name = record.name,
                            Model = modelDict[model_id],
                            Group = !string.IsNullOrEmpty(group_id) ? groupDict[group_id] : null,
                            PermRead = record.perm_read ?? false,
                            PermWrite = record.perm_write ?? false,
                            PermCreate = record.perm_create ?? false,
                            PermUnlink = record.perm_unlink ?? false,
                        });
                    }
                }
                await accessObj.CreateAsync(accessDict.Values);
            }
            await InsertAccesses(module: "sale");

            modelDataList.AddRange(GetModelData(groupDict, "res.groups"));
            modelDataList.AddRange(GetModelData(ruleDict, "ir.rule"));
            await modelDataObj.CreateAsync(modelDataList);
        }

        private IEnumerable<IRModelData> GetModelData<T>(IDictionary<string, T> dict, string model)
        {
            var list = new List<IRModelData>();
            foreach(var item in dict)
            {
                var key = item.Key;
                var arr = key.Split(".");
                var value = item.Value;
                var resId = value.GetType().GetProperty("Id").GetValue(value);
                list.Add(new IRModelData
                {
                    Module = arr[0],
                    Name = arr[1],
                    Model = model,
                    ResId = resId.ToString()
                });
            }
            return list;
        }

        private IDictionary<string, IrModuleCategory> GetModuleCategoryDict()
        {
            var xml_file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ir_module_category_data.xml");
            if (!File.Exists(xml_file_path))
                return new Dictionary<string, IrModuleCategory>();
            var dict = new Dictionary<string, IrModuleCategory>();
            XmlDocument doc = new XmlDocument();

            doc.Load(xml_file_path);
            var records = doc.GetElementsByTagName("record");
            var module = "base";
            for (var i = 0; i < records.Count; i++)
            {
                XmlElement record = (XmlElement)records[i];
                var model = record.GetAttribute("model");
                var id = module + "." + record.GetAttribute("id");
                if (model == "ir.module.category")
                {
                    var categ = new IrModuleCategory();
                    var fields = record.GetElementsByTagName("field");
                    for (var j = 0; j < fields.Count; j++)
                    {
                        XmlElement field = (XmlElement)fields[j];
                        var field_name = field.GetAttribute("name");
                        if (field_name == "name")
                            categ.Name = field.InnerText;
                        else if (field_name == "sequence")
                            categ.Sequence = Convert.ToInt32(field.InnerText);
                        else if (field_name == "visible")
                            categ.Visible = Convert.ToInt32(field.GetAttribute("eval")) != 0;
                    }
                    dict.Add(id, categ);
                }
            }

            return dict;
        }

        private IDictionary<string, IRModel> GetModelDict()
        {
            var dict = new Dictionary<string, IRModel>();
            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ir_model_data.csv");
            if (!File.Exists(file_path))
                return dict;
            using (TextReader reader = File.OpenText(file_path))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.BadDataFound = null;
                var records = csv.GetRecords<IRModelCsvLine>().ToList();
                foreach (var record in records)
                {
                    var model = new IRModel
                    {
                        Model = record.model,
                        Name = record.name,
                        Transient = record.transient
                    };
                    dict.Add(record.id, model);
                }
            }

            return dict;
        }

        public IDictionary<Guid, IList<ResGroup>> _GetTransImplied(IEnumerable<Guid> ids)
        {
            var self = SearchQuery(x => ids.Contains(x.Id)).ToList();
            return _GetTransImplied(self);
        }

        public IDictionary<Guid, IList<ResGroup>> _GetTransImplied(IList<ResGroup> groups)
        {
            var memo = new Dictionary<Guid, IList<ResGroup>>();
            var res = new Dictionary<Guid, IList<ResGroup>>();

            IList<ResGroup> ComputedSet(ResGroup g)
            {
                g = SearchQuery(x => x.Id == g.Id).Include(x => x.ImpliedRels)
                    .Include("ImpliedRels.H").FirstOrDefault();
                if (!memo.ContainsKey(g.Id))
                {
                    var gs = g.ImpliedRels.Select(s => s.H).ToList();
                    memo.Add(g.Id, gs);
                    foreach (var h in gs.ToList())
                    {
                        foreach (var s in ComputedSet(h))
                        {
                            if (!memo[g.Id].Contains(s))
                                memo[g.Id].Add(s);
                        }
                    }
                }
                return memo[g.Id];
            }

            foreach (var g in groups)
            {
                if (!res.ContainsKey(g.Id))
                    res.Add(g.Id, new List<ResGroup>());
                res[g.Id] = ComputedSet(g);
            }

            return res;
        }
    }
}
