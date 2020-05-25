using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareRuleService : BaseService<TCareRule>, ITCareRuleService
    {
        private readonly IMapper _mapper;

        public TCareRuleService(IAsyncRepository<TCareRule> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<object> GetDisplay(Guid id)
        {
            var propObj = GetService<ITCarePropertyService>();
            var rule = await SearchQuery(x => x.Id == id).Include(x => x.Properties).FirstOrDefaultAsync();
            if (rule.Type == "birthday")
            {
                return _GenerateVM<TCareRuleBirthdayDisplay>(rule);
            }

            return null;
        }

        public T _GenerateVM<T>(TCareRule self)
        {
            var propObj = GetService<ITCarePropertyService>();
            var res = _mapper.Map<T>(self);
            foreach (var property in self.Properties)
            {
                var value = propObj.GetValue(property);
                res.GetType().GetProperty(property.Name).SetValue(res, value);
            }

            return res;
        }

        public async Task<TCareRule> CreateRule(TCareRuleSave val)
        {
            var rule = _mapper.Map<TCareRule>(val);
            return await CreateAsync(rule);
        }

        public void _UpdatePropValues(TCareProperty prop, object value)
        {
            if (prop.Type == "Text")
                prop.ValueText = Convert.ToString(value);
            else if (prop.Type == "Decimal")
                prop.ValueDecimal = Convert.ToDecimal(value);
            else if (prop.Type == "Double")
                prop.ValueDouble = Convert.ToDouble(value);
            else if (prop.Type == "DateTime")
                prop.ValueDateTime = Convert.ToDateTime(value);
            else if (prop.Type == "Integer")
                prop.ValueInteger = Convert.ToInt32(value);
        }

        public async Task UpdateBirthdayRule(Guid id, TCareRuleBirthdayUpdate val)
        {
            var rule = await SearchQuery(x => x.Id == id).Include(x => x.Properties).FirstOrDefaultAsync();
            var type_dict = new Dictionary<string, string>()
            {
                { "BeforeDays", "Integer" }
            };

            foreach(var prop in val.GetType().GetProperties())
            {
                var property = rule.Properties.FirstOrDefault(x => x.Name == prop.Name);
                var value = prop.GetValue(val);
                if (property == null)
                {
                    property = new TCareProperty() { Name = prop.Name, Type = type_dict[prop.Name] };
                    _UpdatePropValues(property, value);

                    rule.Properties.Add(property);
                }
                else
                {
                    _UpdatePropValues(property, value);
                }
            }

            await UpdateAsync(rule);
        }
    }
}
