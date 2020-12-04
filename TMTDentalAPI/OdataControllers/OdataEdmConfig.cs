using ApplicationCore.Entities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    public static class OdataEdmConfig
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            #region Partners 
            builder.EntitySet<PartnerViewModel>("Partners");

            builder.EntityType<PartnerViewModel>()
              .Collection
              .Function("GetView")
              .ReturnsCollection<GridPartnerViewModel>();

            builder.EntityType<PartnerViewModel>()
            .Function("GetDisplay")
            .Returns<PartnerDisplay>();

            builder.EntityType<PartnerViewModel>()
            .Function("GetSaleOrders")
            .ReturnsCollectionFromEntitySet<SaleOrderViewModel>("SaleOrders");

            #endregion

            #region PartnerCategories
            builder.EntitySet<PartnerCategoryViewModel>("PartnerCategories");

            #endregion

            #region Products
            builder.EntitySet<ProductViewModel>("Products");
            #endregion

            #region Employees
            builder.EntitySet<EmployeeViewModel>("Employees");

            #endregion

            #region Teeth
            builder.EntitySet<ToothViewModel>("Teeth");

            #endregion

            #region ToothCategories
            builder.EntitySet<ToothCategoryViewModel>("ToothCategories");

            #endregion

            #region FacebookUserProfiles
            builder.EntitySet<FacebookUserProfile>("FacebookUserProfiles");
            builder.EntityType<FacebookUserProfile>()
             .Collection
             .Function("GetView")
             .ReturnsCollection<FacebookUserProfileBasic>();
            #endregion

            #region IRSequences
            builder.EntitySet<IRSequenceViewModel>("IRSequences");

            #endregion

            #region SaleOrders
            builder.EntitySet<SaleOrderViewModel>("SaleOrders");

            builder.EntityType<SaleOrderViewModel>()
             .Function("GetDisplay")
             .Returns<SaleOrderDisplay>();

            builder.EntityType<SaleOrderViewModel>()
            .Function("GetDotKhamListIds")
            .Returns<bool>();

            builder.EntityType<SaleOrderViewModel>()
                .Collection
             .Function("DefaultGet")
             .Returns<SaleOrderDisplay>();

            builder.EntityType<SaleOrderViewModel>()
             .Function("GetSaleOrderLines")
             .Returns<SaleOrderDisplay>();

            builder.EntityType<SaleOrderViewModel>()
            .Function("GetDotKhamStepByOrderLine")
            .Returns<SaleOrderLineBasicViewModel>();
            #endregion

            #region SaleOrderLines
            builder.EntitySet<SaleOrderLineViewModel>("SaleOrderLines");

            #endregion

            #region DotKhamSteps
            builder.EntitySet<DotmKhamStepVM>("DotKhamSteps");

            builder.EntityType<DotmKhamStepVM>()
                .Collection
                .Action("ActionDone");

            builder.EntityType<DotmKhamStepVM>()
                .Collection
                .Action("UpdateList");

            #endregion

            #region SalaryPayments
            builder.EntitySet<SalaryPaymentVm>("SalaryPayments");


            builder.EntityType<SalaryPaymentVm>()
                      .Collection
                   .Action("ActionConfirm")
                    .Returns<bool>();

            builder.EntityType<SalaryPaymentVm>()
                     .Collection
                  .Action("ActionCancel")
                  .Returns<bool>();

            builder.EntityType<SalaryPaymentVm>()
                   .Collection
                .Action("CreateMultiSalaryPayment")
                 .Returns<bool>();

            builder.EntityType<SalaryPaymentVm>()
                   .Collection
                .Action("PrintSalaryPayment")
                 .Returns<bool>();

            builder.EntityType<SalaryPaymentVm>()
               .Collection
            .Action("DefaulCreateBy")
            .ReturnsCollection<SalaryPaymentDisplay>();
            #endregion

            #region AccountJournal
            builder.EntitySet<AccountJournalViewModel>("AccountJournals");
            #endregion

            #region DotKhams
            builder.EntitySet<DotKhamVm>("DotKhams");

            builder.EntityType<DotKhamVm>()
                    .Collection
                 .Action("GetAllDotKhamForSaleOrder")
                 .Returns<bool>();

           builder.EntityType<DotKhamVm>()
          .Function("GetInfo")
          .Returns<DotKhamDisplayVm>();



            #endregion

            #region AccountPayments
            builder.EntitySet<AccountPaymentViewModel>("AccountPayments");

            builder.EntityType<AccountPaymentViewModel>()
                .Function("GetPrint")
                .Returns<PrintData>();
            #endregion

            #region ComplexType
            builder.ComplexType<ApplicationUserSimple>();
            builder.ComplexType<EmployeeBasic>();
            builder.ComplexType<PartnerDisplay>();
            builder.ComplexType<PartnerInfoVm>();
            builder.ComplexType<HistorySimple>();
            builder.ComplexType<ProductPricelistBasic>();
            builder.ComplexType<PartnerCategoryBasic>();
            builder.ComplexType<SaleOrderDisplay>();
            builder.ComplexType<SaleOrderLineDisplay>();
            builder.ComplexType<PartnerSimple>();
            builder.ComplexType<ToothDisplay>();
            builder.ComplexType<DotKhamStepBasic>();
            builder.ComplexType<ToothCategoryBasic>();
            builder.ComplexType<SalaryPaymentDisplay>();
            builder.ComplexType<EmployeeSimple>();
            builder.ComplexType<AccountJournalSimple>();
            builder.ComplexType<PartnerImageBasic>();
            builder.ComplexType<DotKhamDisplay>();
            builder.ComplexType<DotKhamLineDisplay>();
            builder.ComplexType<ProductSimple>();
            builder.ComplexType<SaleOrderLineDisplay>();
            builder.ComplexType<DotKhamDisplayVm>();
            builder.ComplexType<PartnerImageDisplay>();



            #endregion





            return builder.GetEdmModel();
        }
    }
}
