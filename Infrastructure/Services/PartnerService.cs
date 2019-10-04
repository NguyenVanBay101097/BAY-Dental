using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PartnerService : BaseService<Partner>, IPartnerService
    {
        private readonly IMapper _mapper;
        public PartnerService(IAsyncRepository<Partner> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<Partner>> GetPagedResultAsync(int offset, int limit, string search = "", string searchBy = "", bool? customer = null)
        {
            //Tìm kiếm partner search theo tên hoặc mã khách hàng, hoặc theo số điện thoại
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(search))
            {
                switch (searchBy)
                {
                    case "name_ref":
                        {
                            query = query.Where(x => x.Name.Contains(search) || x.Ref.Contains(search));
                            break;
                        }
                    case "phone":
                        {
                            query = query.Where(x => x.Phone.Contains(search));
                            break;
                        }
                    default:
                        {
                            query = query.Where(x => x.Name.Contains(search) || x.Ref.Contains(search));
                            break;
                        }
                }
            }

            if (customer.HasValue)
                query = query.Where(x => x.Customer == customer.Value);

            query = query.OrderBy(x => x.Name); //default order by
            query = query.Skip(offset).Take(limit);

            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<Partner>(totalItems, offset, limit)
            {
                Items = items
            };
        }

        public async Task<Partner> GetPartnerForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.PartnerPartnerCategoryRels)
                .Include(x => x.Employees)
                .Include("PartnerPartnerCategoryRels.Category")
                .FirstOrDefaultAsync();
        }

        public string GetFormatAddress(Partner partner)
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(partner.Street))
                list.Add(partner.Street);
            if (!string.IsNullOrEmpty(partner.WardName))
                list.Add(partner.WardName);
            if (!string.IsNullOrEmpty(partner.DistrictName))
                list.Add(partner.DistrictName);
            if (!string.IsNullOrEmpty(partner.CityName))
                list.Add(partner.CityName);
            var res = string.Join(", ", list);
            return res;
        }

        public string GetGenderDisplay(Partner partner)
        {
            switch (partner.Gender)
            {
                case "female":
                    return "Nữ";
                case "male":
                    return "Nam";
                case "other":
                    return "Khác";
                default:
                    return string.Empty;
            }
        }

        public async Task<PagedResult2<PartnerBasic>> GetPagedResultAsync(PartnerPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();
            var totalItems = await query.CountAsync();

            var partnerList = new List<PartnerBasic>();
            foreach (var i in items)
            {
                var p = new PartnerBasic();
                var list = new List<string>();
                p = _mapper.Map<PartnerBasic>(i);
                if (!string.IsNullOrEmpty(i.Street))
                    list.Add(i.Street);
                if (!string.IsNullOrEmpty(i.WardName))
                    list.Add(i.WardName);
                if (!string.IsNullOrEmpty(i.DistrictName))
                    list.Add(i.DistrictName);
                if (!string.IsNullOrEmpty(i.CityName))
                    list.Add(i.CityName);
                if (list.Count > 0)
                {
                    p.Address = String.Join(", ", list);
                }
                partnerList.Add(p);
            }
            return new PagedResult2<PartnerBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = partnerList
            };
        }

        public override async Task<Partner> CreateAsync(Partner entity)
        {
            if (string.IsNullOrEmpty(entity.Ref) || entity.Ref == "/")
            {
                var sequenceService = (IIRSequenceService)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(IIRSequenceService));
                if (entity.Customer == true)
                {
                    entity.Ref = await sequenceService.NextByCode("customer");
                    if (string.IsNullOrEmpty(entity.Ref))
                    {
                        await InsertCustomerSequence();
                        entity.Ref = await sequenceService.NextByCode("customer");
                    }
                }
                else if (entity.Supplier == true)
                {
                    entity.Ref = await sequenceService.NextByCode("supplier");
                    if (string.IsNullOrEmpty(entity.Ref))
                    {
                        await InsertSupplierSequence();
                        entity.Ref = await sequenceService.NextByCode("supplier");
                    }
                }
            }

            entity.DisplayName = _NameGet(entity);
            return await base.CreateAsync(entity);
        }

        private async Task InsertCustomerSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "customer",
                Name = "Mã khách hàng",
                Prefix = "KH",
                Padding = 5,
            });
        }

        private async Task InsertSupplierSequence()
        {
            var seqObj = GetService<IIRSequenceService>();
            await seqObj.CreateAsync(new IRSequence
            {
                Code = "supplier",
                Name = "Mã nhà cung cấp",
                Prefix = "NCC",
                Padding = 5,
            });
        }

        private string _NameGet(Partner partner)
        {
            var name = partner.Name;
            if (!string.IsNullOrEmpty(partner.Ref))
                name = "[" + partner.Ref + "] " + name;
            return name;
        }

        public override Task UpdateAsync(Partner entity)
        {
            entity.DisplayName = _NameGet(entity);
            return base.UpdateAsync(entity);
        }

        public async Task<IEnumerable<PartnerSimple>> SearchAutocomplete(string filter = "", bool? customer = null)
        {
            var res = await SearchQuery(domain: x => (string.IsNullOrEmpty(filter) || x.Name.Contains(filter)) && (!customer.HasValue || x.Customer == customer))
                .Select(x => new PartnerSimple
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<PartnerSimple>> SearchPartnersCbx(PartnerPaged val)
        {
            var partners = await GetQueryPaged(val).ToListAsync();
            return _mapper.Map<IEnumerable<PartnerSimple>>(partners);
        }

        private IQueryable<Partner> GetQueryPaged(PartnerPaged val)
        {
            var query = SearchQuery();
            if (val.Customer.HasValue)
                query = query.Where(x => x.Customer == val.Customer);
            if (val.Employee.HasValue)
                query = query.Where(x => x.Employee == val.Employee);
            if (val.Supplier.HasValue)
                query = query.Where(x => x.Supplier == val.Supplier);
            if (!string.IsNullOrEmpty(val.SearchNamePhoneRef))
                query = query.Where(x => x.Name.Contains(val.SearchNamePhoneRef) || x.NameNoSign.Contains(val.SearchNamePhoneRef) 
                || x.Ref.Contains(val.SearchNamePhoneRef) || x.Phone.Contains(val.SearchNamePhoneRef));

            //query = query.Skip(val.Offset).Take(val.Limit);
            query = query.OrderByDescending(s => s.DateCreated);
            return query;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null) throw new Exception("File is null");

            //format đường dẫn cố định
            var suffixPath = "assets/images/uploads/files/{0}/{1:yyyy-MM}";
            //đường dẫn upload trên local
            var prefixPath = "C:/Users/TPOS-002/Documents/Visual Studio 2019/TMTDental Branches/TMTDentalAPI-quan/TMTDentalAPI/ClientApp/src";
            //Tên nha khoa
            var dentalName = "Nha Khoa ABC";

            var relativePath = string.Format(suffixPath,dentalName , DateTime.Now).Trim('/');
            var uploadPath = Path.Combine(prefixPath, relativePath).Replace("\\", "/");

                var fileName = file.FileName;

                int index = 0;

                //Ten file ton tai ??
                while (System.IO.File.Exists(Path.Combine(uploadPath, fileName)))
                {
                    index++;
                    fileName = string.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(file.FileName), index, Path.GetExtension(file.FileName));
                }
                //Folder ton tai ??
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                var newPath = Path.Combine(uploadPath, fileName).Replace("\\", "/");
                //Chep file vao thu muc
                using (var fileStream = new FileStream(newPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

            var returnPath = Path.Combine("../../../", relativePath, fileName).Replace("\\", "/");
            return returnPath;
        }

        public async Task<PartnerInfoViewModel> GetInfo(Guid id)
        {
            var partner = await SearchQuery(x => x.Id == id)
                .Include(x => x.PartnerPartnerCategoryRels)
                .Include("PartnerPartnerCategoryRels.Category")
                .FirstOrDefaultAsync();
            var res = _mapper.Map<PartnerInfoViewModel>(partner);
            res.Address = GetFormatAddress(partner);
            res.Gender = GetGenderDisplay(partner);
            res.Categories = string.Join(", ", partner.PartnerPartnerCategoryRels.Select(x => x.Category.Name));
            res.DateOfBirth = GetDateOfBirthDisplay(partner);
            return res;
        }

        private string GetDateOfBirthDisplay(Partner partner)
        {
            return string.Format("{0}/{1}/{2}", partner.BirthDay.HasValue ? partner.BirthDay.Value.ToString() : "",
                partner.BirthMonth.HasValue ? partner.BirthMonth.Value.ToString() : "",
                partner.BirthYear.HasValue ? partner.BirthYear.Value.ToString() : "");
        }

        public async Task<PagedResult2<AccountInvoiceDisplay>> GetCustomerInvoices(AccountInvoicePaged val)
        {
            var accInvObj = GetService<IAccountInvoiceService>();

            var items = await accInvObj.SearchQuery(domain: null, orderBy: x => x.OrderByDescending(s => s.DateInvoice).ThenByDescending(s => s.Number), offSet: val.Offset, limit: val.Limit)
                .Where(x => x.PartnerId == val.PartnerId)
                .Include(x => x.Partner).Include(x => x.User)
                .ToListAsync();
            var totalItems = await accInvObj.SearchQuery(domain: null, orderBy: x => x.OrderByDescending(s => s.DateInvoice).ThenByDescending(s => s.Number)).CountAsync();

            return new PagedResult2<AccountInvoiceDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<AccountInvoiceDisplay>>(items)
            };

        }
    }
}
