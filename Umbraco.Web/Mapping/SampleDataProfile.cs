using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SampleDataProfile : Profile
    {
        public SampleDataProfile()
        {
            CreateMap<Partner, PartnerCustomerXmlSampleDataRecord>();
            CreateMap<PartnerCustomerXmlSampleDataRecord, Partner>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<Partner, PartnerSupplierXmlSampleDataRecord>();
            CreateMap<PartnerSupplierXmlSampleDataRecord, Partner>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<UoM, UomXmlSampleDataRecord>();
            CreateMap<UomXmlSampleDataRecord, UoM>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.CategoryId, x => x.Ignore());

            CreateMap<ProductCategory, ProductCategoryXmlSampleDataRecord>();
            CreateMap<ProductCategoryXmlSampleDataRecord, ProductCategory>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<Product, ProductServiceXmlSampleDataRecord>();
            CreateMap<ProductServiceXmlSampleDataRecord, Product>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.CategId, x => x.Ignore());

            CreateMap<Product, ProductProductXmlSampleDataRecord>();
            CreateMap<ProductProductXmlSampleDataRecord, Product>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.CategId, x => x.Ignore())
                .ForMember(x => x.UOMPOId, x => x.Ignore())
                .ForMember(x => x.UOMId, x => x.Ignore());

            CreateMap<Product, ProductMedicineXmlSampleDataRecord>();
            CreateMap<ProductMedicineXmlSampleDataRecord, Product>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.CategId, x => x.Ignore())
                .ForMember(x => x.UOMId, x => x.Ignore());

            CreateMap<Product, ProductLaboXmlSampleDataRecord>();
            CreateMap<ProductLaboXmlSampleDataRecord, Product>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<LaboBiteJoint, SimpleXmlSampleDataRecord>();
            CreateMap<SimpleXmlSampleDataRecord, LaboBiteJoint>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<LaboBridge, SimpleXmlSampleDataRecord>();
            CreateMap<SimpleXmlSampleDataRecord, LaboBridge>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<LaboFinishLine, SimpleXmlSampleDataRecord>();
            CreateMap<SimpleXmlSampleDataRecord, LaboFinishLine>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<Employee, EmployeeXmlSampleDataRecord>();
            CreateMap<EmployeeXmlSampleDataRecord, Employee>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<LoaiThuChi, LoaiThuChiXmlSampleDataRecord>();
            CreateMap<LoaiThuChiXmlSampleDataRecord, LoaiThuChi>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SaleOrder, SaleOrderXmlSampleDataRecord>()
                .ForMember(x => x.OrderLines, x => x.Ignore());
            CreateMap<SaleOrderXmlSampleDataRecord, SaleOrder>()
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.OrderLines, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SaleOrderLine, SaleOrderLineXmlSampleDataRecord>()
                .ForMember(x=> x.SaleOrderLineToothRels, x=> x.Ignore());
            CreateMap<SaleOrderLineXmlSampleDataRecord, SaleOrderLine>()
                .ForMember(x => x.ProductId, x => x.Ignore())
                .ForMember(x => x.SaleOrderLineToothRels, x => x.Ignore())
                .ForMember(x => x.ToothCategoryId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SaleOrderPayment, SaleOrderPaymentXmlSampleDataRecord>()
                 .ForMember(x => x.JournalLines, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
            CreateMap<SaleOrderPaymentXmlSampleDataRecord, SaleOrderPayment>()
                .ForMember(x => x.JournalLines, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.OrderId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<LaboOrder, LaboOrderXmlSampleDataRecord>()
                 .ForMember(x => x.LaboOrderProductRel, x => x.Ignore())
                .ForMember(x => x.LaboOrderToothRel, x => x.Ignore());
            CreateMap<LaboOrderXmlSampleDataRecord, LaboOrder>()
                .ForMember(x => x.LaboOrderToothRel, x => x.Ignore())
                .ForMember(x => x.LaboOrderProductRel, x => x.Ignore())
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.ProductId, x => x.Ignore())
                .ForMember(x => x.SaleOrderLineId, x => x.Ignore())
                .ForMember(x => x.LaboFinishLineId, x => x.Ignore())
                .ForMember(x => x.LaboBiteJointId, x => x.Ignore())
                .ForMember(x => x.LaboBridgeId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<Appointment, AppointmentXmlSampleDataRecord>()
                 .ForMember(x => x.AppointmentServices, x => x.Ignore());
            CreateMap<AppointmentXmlSampleDataRecord, Appointment>()
                .ForMember(x => x.AppointmentServices, x => x.Ignore())
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.DoctorId, x => x.Ignore())
                .ForMember(x => x.SaleOrderId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<CustomerReceipt, CustomerReceiptXmlSampleDataRecord>()
                 .ForMember(x => x.CustomerReceiptProductRels, x => x.Ignore());
            CreateMap<CustomerReceiptXmlSampleDataRecord, CustomerReceipt>()
                .ForMember(x => x.CustomerReceiptProductRels, x => x.Ignore())
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.DoctorId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<PhieuThuChi, PhieuThuChiXmlSampleDataRecord>();
            CreateMap<PhieuThuChiXmlSampleDataRecord, PhieuThuChi>()
                .ForMember(x => x.JournalId, x => x.Ignore())
                .ForMember(x => x.LoaiThuChiId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<PurchaseOrder, PurchaseOrderXmlSampleDataRecord>()
                 .ForMember(x => x.OrderLines, x => x.Ignore());
            CreateMap<PurchaseOrderXmlSampleDataRecord, PurchaseOrder>()
                .ForMember(x => x.OrderLines, x => x.Ignore())
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.JournalId, x => x.Ignore())
                .ForMember(x => x.PickingTypeId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<PurchaseOrderLine, PurchaseOrderLineXmlSampleDataRecord>();
            CreateMap<PurchaseOrderLineXmlSampleDataRecord, PurchaseOrderLine>()
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.ProductUOMId, x => x.Ignore())
                .ForMember(x => x.ProductId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<StockPicking, StockPickingXmlSampleDataRecord>()
                .ForMember(x => x.MoveLines, x => x.Ignore());
            CreateMap<StockPickingXmlSampleDataRecord, StockPicking>()
                .ForMember(x => x.MoveLines, x => x.Ignore())
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.PickingTypeId, x => x.Ignore())
                .ForMember(x => x.LocationId, x => x.Ignore())
                .ForMember(x => x.LocationDestId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<StockMove, StockMoveXmlSampleDataRecord>();
            CreateMap<StockMoveXmlSampleDataRecord, StockMove>()
                .ForMember(x => x.PickingTypeId, x => x.Ignore())
                .ForMember(x => x.PartnerId, x => x.Ignore())
                .ForMember(x => x.ProductUOMId, x => x.Ignore())
                .ForMember(x => x.ProductId, x => x.Ignore())
                .ForMember(x => x.LocationId, x => x.Ignore())
                .ForMember(x => x.LocationDestId, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
