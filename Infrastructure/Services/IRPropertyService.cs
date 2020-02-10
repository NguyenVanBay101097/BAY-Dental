using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRPropertyService : BaseService<IRProperty>, IIRPropertyService
    {
        //cache tren property cho tung company, res_id, property_name, company_id
        //"product.product,A7621557-6B30-4D30-B43F-A7CF00944FC5,standard_price,4CC95C64-8F12-467B-849B-A7D00072766A"
        private const string CACHE_KEY = "ir.property-{0},{1},{2}";
        private readonly IMyCache _cacheManager;
        private readonly AppTenant _tenant;

        public IRPropertyService(IAsyncRepository<IRProperty> repository, IHttpContextAccessor httpContextAccessor,
            IMyCache cacheManager, ITenant<AppTenant> tenant)
            : base(repository, httpContextAccessor)
        {
            _cacheManager = cacheManager;
            _tenant = tenant?.Value;
        }

        public IDictionary<string, string> TYPE2FIELD
        {
            get
            {
                var dict = new Dictionary<string, string>();
                dict.Add("char", "value_text");
                dict.Add("float", "value_float");
                dict.Add("boolean", "value_integer");
                dict.Add("integer", "value_integer");
                dict.Add("binary", "value_binary");
                dict.Add("many2one", "value_reference");
                dict.Add("date", "value_datetime");
                dict.Add("datetime", "value_datetime");
                dict.Add("selection", "value_text");

                return dict;
            }
        }

        public IRProperty CreateProperty(IRProperty values)
        {
            Create(_UpdateValues(values));
            return values;
        }

        public void WriteProperty(IRProperty values)
        {
            var key = GetCacheKey(values);
            _cacheManager.Remove(key); //clear cache
            Update(_UpdateValues(values));
        }

        public override Task DeleteAsync(IRProperty values)
        {
            var key = GetCacheKey(values);
            _cacheManager.Remove(key); //clear cache
            return base.DeleteAsync(values);
        }

        private IRProperty _UpdateValues(IRProperty values)
        {
            var fieldObj = GetService<IIRModelFieldService>();
            var value = values.Value;
            if (value == null)
                return values;
            var type_ = values.Type;
            if (string.IsNullOrEmpty(type_))
            {
                type_ = values.Field != null ? values.Field.TType : string.Empty;
            }

            var field = TYPE2FIELD.ContainsKey(type_) ? TYPE2FIELD[type_] : string.Empty;
            if (string.IsNullOrEmpty(field))
                throw new Exception("Invalid type");

            if (field == "value_reference")
            {
                if (value is long && values.Field != null)
                    value = string.Format("{0},{1}", values.Field.Relation, value);
                values.ValueReference = (string)value;
            }
            else if (field == "value_float")
            {
                if (value.GetType() == typeof(decimal) || value.GetType() == typeof(decimal?))
                {
                    values.ValueFloat = (double)((decimal)(value ?? 0));
                }
                else if (value.GetType() == typeof(double) || value.GetType() == typeof(double?))
                {
                    values.ValueFloat = (double)(value ?? 0);
                }
            }
            else if (field == "value_text")
            {
                values.ValueText = (string)value;
            }

            return values;
        }

        public void set_multi(string name, string model, IDictionary<string, object> values, object default_value = null,
         Guid? force_company = null)
        {
            /*
             """ Assign the property field `name` for the records of model `model`
            with `values` (dictionary mapping record ids to their value).
            If the value for a given record is the same as the default
            value, the property entry will not be stored, to avoid bloating
            the database.
            If `default_value` is provided, that value will be used instead
            of the computed default value, to determine whether the value
            for a record should be stored or not.
            """
             */

            object clean(object value)
            {
                if (value == null)
                    return value;
                if (value.GetType().IsSubclassOf(typeof(BaseEntity)))
                    return ((BaseEntity)value).Id;
                return value;
            }

            if (values == null)
                return;

            if (default_value == null)
            {
                var domain = _get_domain(name, model, force_company: force_company);
                if (domain == null)
                    return;
                default_value = clean(get(name, model));
            }

            var fieldObj = GetService<IIRModelFieldService>();
            var companyObj = GetService<ICompanyService>();
            //retrieve the properties corresponding to the given record ids
            var field = fieldObj.SearchQuery(x => x.Name == name && x.Model == model).FirstOrDefault();
            var field_id = field != null ? field.Id : (Guid?)null;
            var company_id = force_company ?? CompanyId;
            var res_ids = values.Keys.Select(x => x);
            //invoke stored procedure
            //prepare parameters
            var resIds = new SqlParameter();
            resIds.ParameterName = "ResIds";
            resIds.Value = string.Join(";", res_ids);
            resIds.DbType = DbType.String;

            var fieldId = new SqlParameter();
            fieldId.ParameterName = "FieldId";
            fieldId.Value = field_id.ToString();
            fieldId.DbType = DbType.String;

            var companyId = new SqlParameter();
            companyId.ParameterName = "CompanyId";
            companyId.Value = company_id.ToString();
            companyId.DbType = DbType.String;

            var props = SqlQuery(
                "SearchIrProperty",
                resIds,
                fieldId,
                companyId);

            //modify existing properties
            foreach (var prop in props)
            {
                var id = prop.ResId;
                var value = values.ContainsKey(id) ? clean(values[id]) : null;

                if (Equals(value, default_value))
                {
                    var key = GetCacheKey(prop);
                    _cacheManager.Remove(key); //clear cache
                    Delete(prop);
                }
                else if (!Equals(value, clean(get_by_record(prop))))
                {
                    prop.Value = value;
                    WriteProperty(prop);
                }

                values.Remove(id);
            }

            //create new properties for records that do not have one yet
            foreach (var item in values)
            {
                var res_id = item.Key;
                var value = clean(item.Value);
                if (!Equals(value, default_value))
                {
                    var prop = new IRProperty
                    {
                        Field = field,
                        CompanyId = company_id,
                        ResId = res_id,
                        Name = name,
                        Value = value,
                        Type = field != null ? field.TType : "many2one",
                    };
                    CreateProperty(_UpdateValues(prop));
                }
            }
        }

        private string GetCacheKey(IRProperty prop)
        {
            var key = string.Format(CACHE_KEY, prop.ResId, prop.Name, prop.CompanyId);
            if (_tenant != null)
                key = _tenant.Hostname + "-" + key;
            return key;
        }

        public T get<T>(string name, string model, string res_id = null, Guid? force_company = null)
        {
            var company_id = force_company ?? CompanyId;
            string key = string.Format(CACHE_KEY, res_id, name, company_id);
            if (_tenant != null)
                key = _tenant.Hostname + "-" + key;

            var result = _cacheManager.GetOrCreate<T>(key, entry =>
            {
                var domain = _get_domain(name, model, force_company: force_company);
                if (domain != null)
                {
                    domain = domain.And(new InitialSpecification<IRProperty>(x => x.ResId == res_id));

                    var prop = SearchQuery(domain.AsExpression(), orderBy: x => x.OrderBy(s => s.CompanyId)).FirstOrDefault();
                    if (prop != null)
                    {
                        return (T)get_by_record(prop);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(res_id))
                            return get<T>(name, model); //try get property default
                    }
                }

                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return default(T);
            });

            return result;
        }

        public object get(string name, string model, string res_id = null, Guid? force_company = null)
        {
            var company_id = force_company ?? CompanyId;
            string key = string.Format(CACHE_KEY, res_id, name, company_id);
            if (_tenant != null)
                key = _tenant.Hostname + "-" + key;
            return _cacheManager.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);

                var domain = _get_domain(name, model, force_company: force_company);
                if (domain != null)
                {
                    domain = domain.And(new InitialSpecification<IRProperty>(x => x.ResId == res_id));

                    var prop = SearchQuery(domain.AsExpression(), orderBy: x => x.OrderBy(s => s.CompanyId)).FirstOrDefault();
                    if (prop != null)
                    {
                        return get_by_record(prop);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(res_id))
                            return get(name, model); //try get property default
                    }
                }

                return null;
            });
        }

        public IDictionary<string, object> get_multi(string name, string model, IEnumerable<string> ids, Guid? force_company = null)
        {
            if (!ids.Any())
                return new Dictionary<string, object>();

            var domain = _get_domain(name, model, force_company: force_company);
            if (domain == null)
                return ids.ToDictionary(x => x, x => (object)null);

            //retrieve the values for the given ids and the default value, too
            ids = ids.Distinct();
            var refs = ids.ToDictionary(x => string.Format("{0},{1}", model, x), x => x);
            refs.Add(string.Empty, string.Empty);
            var refs_list = refs.Keys.ToArray();
            domain = domain.And(new InitialSpecification<IRProperty>(x => string.IsNullOrEmpty(x.ResId) || refs_list.Contains(x.ResId)));

            var props = SearchQuery(domain.AsExpression(), orderBy: x => x.OrderBy(s => s.CompanyId));

            var results = new Dictionary<string, object>();
            foreach (var prop in props)
            {
                string id = string.Empty;
                if (!string.IsNullOrEmpty(prop.ResId) && !refs.ContainsKey(prop.ResId))
                    continue;

                if (!string.IsNullOrEmpty(prop.ResId))
                    id = refs[prop.ResId];
                if (string.IsNullOrEmpty(prop.ResId))
                    refs.Remove(string.Empty);
                else
                    refs.Remove(prop.ResId);

                if (!results.ContainsKey(id))
                    results.Add(id, null);
                results[id] = get_by_record(prop);
            }

            var default_value = results.ContainsKey(string.Empty) ? results[string.Empty] : null;
            foreach (var id in ids)
            {
                if (!results.ContainsKey(id))
                    results[id] = default_value;
            }

            return results;
        }

        private object get_by_record(IRProperty self)
        {
            if (self.Type == "char" || self.Type == "text" || self.Type == "selection")
                return self.ValueText;
            else if (self.Type == "float")
                return self.ValueFloat;
            else if (self.Type == "boolean")
                return Convert.ToBoolean(self.ValueInteger ?? 0);
            else if (self.Type == "integer")
                return self.ValueInteger;
            else if (self.Type == "binary")
                return self.ValueBinary;
            else if (self.Type == "many2one")
            {
                if (!string.IsNullOrEmpty(self.ValueReference))
                {
                    var references = self.ValueReference.Split(',');
                    var model = references[0];
                    var resource_id = references[1];
                }
            }
            else if (self.Type == "datetime")
                return self.ValueDateTime;
            else if (self.Type == "date")
                return self.ValueDateTime.HasValue ? self.ValueDateTime.Value.Date : (DateTime?)null;
            return null;
        }

        private ISpecification<IRProperty> _get_domain(string name, string model, Guid? force_company = null)
        {
            var fieldObj = GetService<IIRModelFieldService>();
            var companyObj = GetService<ICompanyService>();

            var res = fieldObj.SearchQuery(x => x.Name == name && x.Model == model).Select(x => x.Id).ToList();
            if (!res.Any())
                return null;
            var field_id = res[0];
            var company_id = force_company ?? CompanyId;
            return new InitialSpecification<IRProperty>(x => x.FieldId == field_id && (!x.CompanyId.HasValue || x.CompanyId == company_id));
        }
    }
}
