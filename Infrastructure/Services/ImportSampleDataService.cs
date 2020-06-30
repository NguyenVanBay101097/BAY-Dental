using ApplicationCore.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ImportSampleDataService : IImportSampleDataService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public ImportSampleDataService(IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task ImportSampleData()
        {
            var file_path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\importSampleData.xml");
            XElement xml = XElement.Load(file_path);
            XmlSerializer serializer = new XmlSerializer(typeof(SampleData));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToString()));
            SampleData sampleData = (SampleData)serializer.Deserialize(memStream);

            var partnerCategObj = GetService<IPartnerCategoryService>();
            var partnerObj = GetService<IPartnerService>();
            var productCategObj = GetService<IProductCategoryService>();
            var productObj = GetService<IProductService>();
            var historyObj = GetService<IHistoryService>();
            var productStepObj = GetService<IProductStepService>();

            var partner_category_dict = new Dictionary<string, PartnerCategory>();
            var partner_dict = new Dictionary<string, Partner>();
            var history_dict = new Dictionary<string, History>();
            var product_category_dict = new Dictionary<string, ProductCategory>();
            var product_dict = new Dictionary<string, Product>();
            var product_step_dict = new Dictionary<string, ProductStep>();

            if (sampleData != null && sampleData.Data != null && sampleData.Data.Record.Count > 0)
            {

                var record = sampleData.Data.Record;

                foreach (var itemRecord in record.ToList())
                {
                    switch (itemRecord.Model)
                    {
                        case "res.partner.category":
                            partner_category_dict.Add(itemRecord.Id,new PartnerCategory());
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        partner_category_dict[itemRecord.Id].Name = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case "res.partner":
                            partner_dict.Add(itemRecord.Id, new Partner());
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        partner_dict[itemRecord.Id].Name = itemField.Text;
                                        break;
                                    case "email":
                                        partner_dict[itemRecord.Id].Email = itemField.Text;
                                        break;
                                    case "phone":
                                        partner_dict[itemRecord.Id].Phone = itemField.Text;
                                        break;
                                    case "gender":
                                        partner_dict[itemRecord.Id].Gender = itemField.Text;
                                        break;
                                    case "birth_day":
                                        partner_dict[itemRecord.Id].BirthDay = Int32.Parse(itemField.Text);
                                        break;
                                    case "birth_month":
                                        partner_dict[itemRecord.Id].BirthMonth = Int32.Parse(itemField.Text);
                                        break;
                                    case "birth_year":
                                        partner_dict[itemRecord.Id].BirthYear = Int32.Parse(itemField.Text);
                                        break;
                                    case "supplier":
                                        partner_dict[itemRecord.Id].Supplier = Boolean.Parse(itemField.Text);
                                        break;
                                    case "customer":
                                        partner_dict[itemRecord.Id].Customer = Boolean.Parse(itemField.Text);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case "product.category":
                            product_category_dict.Add(itemRecord.Id, new ProductCategory());
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        product_category_dict[itemRecord.Id].Name = itemField.Text;
                                        break;
                                    case "type":
                                        product_category_dict[itemRecord.Id].Type = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        case "product.product":
                            product_dict.Add(itemRecord.Id, new Product());
                            var uom = await GetService<IUoMService>().SearchQuery(x => x.Name.Contains("Cái")).FirstOrDefaultAsync();
                            product_dict[itemRecord.Id].UOMId = uom.Id;
                            product_dict[itemRecord.Id].UOMPOId = uom.Id;
                            product_dict[itemRecord.Id].CompanyId = CompanyId;
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        product_dict[itemRecord.Id].Name = itemField.Text;
                                        break;
                                    case "list_price":
                                        product_dict[itemRecord.Id].ListPrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "labo_price":
                                        product_dict[itemRecord.Id].LaboPrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "purchase_price":
                                        product_dict[itemRecord.Id].PurchasePrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "is_labo":
                                        product_dict[itemRecord.Id].IsLabo = Boolean.Parse(itemField.Text);
                                        break;
                                    case "purchase_ok":
                                        product_dict[itemRecord.Id].PurchaseOK = Boolean.Parse(itemField.Text);
                                        break;
                                    case "type":
                                        product_dict[itemRecord.Id].Type = itemField.Text;
                                        break;
                                    case "type_2":
                                        product_dict[itemRecord.Id].Type2 = itemField.Text;
                                        break;
                                    case "categ_id":
                                        product_dict[itemRecord.Id].Categ = product_category_dict[itemField.Ref];
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;

                        case "product.step":
                            product_step_dict.Add(itemRecord.Id, new ProductStep());
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                if (itemField.Name == "name")
                                    product_step_dict[itemRecord.Id].Name = itemField.Text;
                                if (itemField.Name == "product_id")
                                    product_step_dict[itemRecord.Id].Product = product_dict[itemField.Ref];
                            }
                            break;

                        case "history":
                            history_dict.Add(itemRecord.Id, new History());
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        history_dict[itemRecord.Id].Name = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                }

                await partnerCategObj.CreateAsync(partner_category_dict.Values.ToList());
                await partnerObj.CreateAsync(partner_dict.Values.ToList());
                await historyObj.CreateAsync(history_dict.Values.ToList());
                await productCategObj.CreateAsync(product_category_dict.Values.ToList());
                await productObj.CreateAsync(product_dict.Values.ToList());
                await productStepObj.CreateAsync(product_step_dict.Values.ToList());
            }
        }

        protected T GetService<T>()
        {
            return (T)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(T));
        }

        protected Guid CompanyId
        {
            get
            {
                if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                    return Guid.Empty;
                var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "company_id");
                return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
            }
        }
    }

}
