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
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSampleDataXml));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xml.ToString()));
            ImportSampleDataXml sampleData = (ImportSampleDataXml)serializer.Deserialize(memStream);

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
                var uomObj = GetService<IUoMService>();
                var uom = await uomObj.DefaultUOM();
                foreach (var itemRecord in record.ToList())
                {
                    switch (itemRecord.Model)
                    {
                        case "res.partner.category":
                            var partnerCateg = new PartnerCategory();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        partnerCateg.Name = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            partner_category_dict.Add(itemRecord.Id, partnerCateg);
                            break;

                        case "res.partner":
                            var partner = new Partner();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                       partner.Name = itemField.Text;
                                        break;
                                    case "email":
                                       partner.Email = itemField.Text;
                                        break;
                                    case "phone":
                                       partner.Phone = itemField.Text;
                                        break;
                                    case "gender":
                                       partner.Gender = itemField.Text;
                                        break;
                                    case "birth_day":
                                       partner.BirthDay = Int32.Parse(itemField.Text);
                                        break;
                                    case "birth_month":
                                       partner.BirthMonth = Int32.Parse(itemField.Text);
                                        break;
                                    case "birth_year":
                                       partner.BirthYear = Int32.Parse(itemField.Text);
                                        break;
                                    case "supplier":
                                       partner.Supplier = Boolean.Parse(itemField.Text);
                                        break;
                                    case "customer":
                                       partner.Customer = Boolean.Parse(itemField.Text);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            partner_dict.Add(itemRecord.Id, partner);
                            break;

                        case "product.category":

                            var productCategory = new ProductCategory();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        productCategory.Name = itemField.Text;
                                        break;
                                    case "type":
                                        productCategory.Type = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            product_category_dict.Add(itemRecord.Id, productCategory);
                            break;

                        case "product.product":
                            var product = new Product();
                            product.UOMId = uom.Id;
                            product.UOMPOId = uom.Id;
                            product.CompanyId = CompanyId;
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        product.Name = itemField.Text;
                                        break;
                                    case "list_price":
                                        product.ListPrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "labo_price":
                                        product.LaboPrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "purchase_price":
                                        product.PurchasePrice = Decimal.Parse(itemField.Text);
                                        break;
                                    case "is_labo":
                                        product.IsLabo = Boolean.Parse(itemField.Text);
                                        break;
                                    case "purchase_ok":
                                        product.PurchaseOK = Boolean.Parse(itemField.Text);
                                        break;
                                    case "type":
                                        product.Type = itemField.Text;
                                        break;
                                    case "type_2":
                                        product.Type2 = itemField.Text;
                                        break;
                                    case "categ_id":
                                        product.Categ = product_category_dict[itemField.Ref];
                                        break;
                                    default:
                                        break;
                                }
                            }
                            product_dict.Add(itemRecord.Id, product);
                            break;

                        case "product.step":
                            var productStep = new ProductStep();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                if (itemField.Name == "name")
                                    productStep.Name = itemField.Text;
                                if (itemField.Name == "product_id")
                                    productStep.Product = product_dict[itemField.Ref];
                            }
                            product_step_dict.Add(itemRecord.Id, productStep);
                            break;

                        case "history":
                            var history = new History();
                            foreach (var itemField in itemRecord.Field.ToList())
                            {
                                switch (itemField.Name)
                                {
                                    case "name":
                                        history.Name = itemField.Text;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            history_dict.Add(itemRecord.Id, history);
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
