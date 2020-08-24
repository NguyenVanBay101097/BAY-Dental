using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRModelAccessService : BaseService<IRModelAccess>, IIRModelAccessService
    {
        private const string CACHE_KEY_FORMAT = "{0}-ir.model.access-{1}-{2}-{3}";
        private readonly CatalogDbContext _context;
        private readonly AppTenant _tenant;
        public string[] TransientModels = new string[] {
            "AccountAccount",
            "AccountAccountType",
            "AccountFullReconcile",
            "AccountJournal",
            "AccountMove",
            "AccountMoveLine",
            "AccountPartialReconcile",
            "StockMove",
            "StockLocation",
            "StockPickingType",
            "StockQuant",
            "StockWarehouse",
            "IrAttachment",
            "IRSequence",
            "UoM",
            "ProductStep",
            "AccountRegisterPayment",
            "AccountInvoice",
            "AccountInvoiceLine",
            "DotKhamStep",
            "ToothCategory",
            "Tooth",
            "ProductPricelistItem",
            "SaleOrderLine",
            "MailMessage",
            "CardHistory",
            "IRModelData",
            "LaboOrderLine",
            "PromotionRule",
            "IrConfigParameter",
            "SaleOrderServiceCardCardRel",
            "ServiceCardOrderLine",
            "IRModelData",
            "IRModelField",
            "IRProperty",
            "UoMCategory",
            "AccountFinancialReport",
            "SaleOrderLinePartnerCommission",
            "CommissionSettlement",
            "CommissionProductRule"
        };
        private IMyCache _cache;
        public IRModelAccessService(IAsyncRepository<IRModelAccess> repository, IHttpContextAccessor httpContextAccessor,
            CatalogDbContext context, IMyCache cache, ITenant<AppTenant> tenant)
        : base(repository, httpContextAccessor)
        {
            _context = context;
            _cache = cache;
            _tenant = tenant?.Value;
        }

        public bool Check(string model, string mode = "Read", bool raiseException = true)
        {
            if (string.IsNullOrEmpty(UserId))
                return true;

            var CACHE_KEY = "ir.model.access-{0}-{1}-{2}";
            var key = string.Format(CACHE_KEY, UserId, model, mode);
            if (_tenant != null)
                key = _tenant.Hostname + "-" + key;

            var result = _cache.GetOrCreate(key, entry =>
            {
                if (IsUserRoot)
                    return true;

                var modes = new string[] { "Read", "Write", "Create", "Unlink" };
                if (!modes.Contains(mode))
                    throw new Exception("Invalid access mode");

                //transient models no nead check access right
                if (TransientModels.Contains(model))
                    return true;

                var userId = UserId;
                bool r = false;
                if (mode == "Read")
                {
                    var access = _context.ModelAccessReports.FromSqlInterpolated($"SELECT * from dbo.model_access_report where Model={model} and UserId={userId} and Active=1 and PermRead=1").FirstOrDefault();
                    r = access != null;
                }
                else if (mode == "Create")
                {
                    var access = _context.ModelAccessReports.FromSqlInterpolated($"SELECT * from dbo.model_access_report where Model={model} and UserId={userId} and Active=1 and PermCreate=1").FirstOrDefault();
                    r = access != null;
                }
                else if (mode == "Write")
                {
                    var access = _context.ModelAccessReports.FromSqlInterpolated($"SELECT * from dbo.model_access_report where Model={model} and UserId={userId} and Active=1 and PermWrite=1").FirstOrDefault();
                    r = access != null;
                }
                else if (mode == "Unlink")
                {
                    var access = _context.ModelAccessReports.FromSqlInterpolated($"SELECT * from dbo.model_access_report where Model={model} and UserId={userId} and Active=1 and PermUnlink=1").FirstOrDefault();
                    r = access != null;
                }

                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                return r;
            });

            if (!result && raiseException)
            {
                var msgHeads = new Dictionary<string, string>();
                msgHeads.Add("Read", "Xin lỗi, bạn không được phép xem tài liệu này.");
                msgHeads.Add("Write", "Xin lỗi, bạn không được phép chỉnh sửa tài liệu này.");
                msgHeads.Add("Create", "Xin lỗi, bạn không được phép tạo loại tài liệu này.");
                msgHeads.Add("Unlink", "Xin lỗi, bạn không được phép xóa tài liệu này.");

                var irModel = _context.IRModels.Where(x => x.Model == model).FirstOrDefault();
                var msg = msgHeads[mode];
                if (irModel != null)
                {
                    msg += " " + irModel.Name;
                }
                else
                {
                    msg += " " + model;
                }
                throw new Exception(msg);
            }

            return result;
        }

        public async Task<IDictionary<string, IRModelAccess>> InsertIfNotExist(IDictionary<string, IRModelAccess> dict)
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var new_dict = new Dictionary<string, IRModelAccess>();
            foreach (var item in dict)
            {
                var reference = item.Key;
                var model = await modelDataObj.GetRef<IRModelAccess>(reference);
                if (model == null)
                {
                    model = item.Value;
                    await CreateAsync(model);

                    var referenceSplit = reference.Split(".");

                    var modelData = new IRModelData()
                    {
                        Model = "ir.model",
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
    }
}
