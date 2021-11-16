using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Umbraco.Web.Models.ContentEditing
{
    [XmlRoot(ElementName = "field")]
    public class Field
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "ref")]
        public string Ref { get; set; }
        [XmlElement(ElementName = "record")]
        public List<Record> Records { get; set; } = new List<Record>();
    }

    [XmlRoot(ElementName = "record")]
    public class Record
    {
        [XmlElement(ElementName = "field")]
        public List<Field> Field { get; set; } = new List<Field>();
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "model")]
        public string Model { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {
        [XmlElement(ElementName = "record")]
        public List<Record> Record { get; set; } = new List<Record>();
    }

    [XmlRoot(ElementName = "data")]
    public class ListRecord
    {
        [XmlElement(ElementName = "record")]
        public List<Record> Record { get; set; } = new List<Record>();
    }

    [XmlRoot(ElementName = "sampleData")]
    public class ImportSampleDataXml
    {
        [XmlElement(ElementName = "data")]
        public Data Data { get; set; }
    }

    [XmlRoot(ElementName = "Data")]
    public class XmlSampleData<T>
    {
        [XmlElement(ElementName = "Record")]
        public List<T> Records { get; set; } = new List<T>();
    }

    [XmlType("Record")]
    public class ProductCategoryXmlSampleDataRecord
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }

    [XmlType("Record")]
    public class ProductServiceXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategId { get; set; }
        public decimal ListPrice { get; set; }
        public decimal StandardPrice { get; set; }
        public bool IsLabo { get; set; }
        public decimal? LaboPrice { get; set; }
        public string Firm { get; set; }
    }

    [XmlType("Record")]
    public class ProductProductXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategId { get; set; }
        public decimal? MinInventory { get; set; }
        public string UOMPOId { get; set; }
        public string UOMId { get; set; }
        public decimal? PurchasePrice { get; set; }
    }

    [XmlType("Record")]
    public class ProductMedicineXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CategId { get; set; }
        public decimal? MinInventory { get; set; }
        public decimal ListPrice { get; set; }
        public decimal StandardPrice { get; set; }
        public string UOMId { get; set; }
    }

    [XmlType("Record")]
    public class ProductLaboXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [XmlType("Record")]
    public class SimpleXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [XmlType("Record")]
    public class PartnerCustomerXmlSampleDataRecord
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string Street { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public string Id { get; set; }
        public int DateRound { get; set; } // để set date.now - this
    }

    [XmlType("Record")]
    public class PartnerSupplierXmlSampleDataRecord
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Street { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public string Id { get; set; }
        public int DateRound { get; set; } // để set date.now - this
    }

    [XmlType("Record")]
    public class UomXmlSampleDataRecord
    {
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string Id { get; set; }
    }

    [XmlRoot("Record")]
    [XmlType("Record")]
    public class EmployeeXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsDoctor { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? StartWorkDate { get; set; }
        public string IdentityCard { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }
    }


    [XmlType("Record")]
    public class LoaiThuChiXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

    }

    [XmlType("Record")]
    public class SaleOrderXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string PartnerId { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        public int TimeHour { get; set; }
        public int TimeMinute { get; set; }
        [XmlArray("OrderLines")]
        [XmlArrayItem("Record")]
        public List<SaleOrderLineXmlSampleDataRecord> OrderLines { get; set; } = new List<SaleOrderLineXmlSampleDataRecord>();
    }

    [XmlRoot("Record")]
    public class SaleOrderLineXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal ProductUOMQty { get; set; }
        public string ProductId { get; set; }
        public string Diagnostic { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        public string ToothCategoryId { get; set; }
        [XmlArray("SaleOrderLineToothRels")]
        [XmlArrayItem("Record")]
        public List<SaleOrderLineToothRelXmlSampleDataRecord> SaleOrderLineToothRels { get; set; } = new List<SaleOrderLineToothRelXmlSampleDataRecord>();
    }

    [XmlRoot("Record")]
    public class SaleOrderLineToothRelXmlSampleDataRecord
    {
        public string ToothId { get; set; }
    }

    [XmlType("Record")]
    public class SaleOrderPaymentXmlSampleDataRecord
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        [XmlArray("JournalLines")]
        [XmlArrayItem("Record")]
        public List<SaleOrderPaymentJournalLineXmlSampleDataRecord> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLineXmlSampleDataRecord>();
        [XmlArray("Lines")]
        [XmlArrayItem("Record")]
        public List<SaleOrderPaymentHistoryLineXmlSampleDataRecord> Lines { get; set; } = new List<SaleOrderPaymentHistoryLineXmlSampleDataRecord>();
    }

    [XmlRoot("Record")]
    [XmlType("JournalLines")]
    public class SaleOrderPaymentJournalLineXmlSampleDataRecord
    {
        public string JournalId { get; set; }
        public decimal Amount { get; set; }
    }

    [XmlRoot("Record")]
    [XmlType("Lines")]
    public class SaleOrderPaymentHistoryLineXmlSampleDataRecord
    {
        public string SaleOrderLineId { get; set; }
        public decimal Amount { get; set; }
    }

    [XmlType("Record")]
    public class LaboOrderXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string PartnerId { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        public string ProductId { get; set; }
        public string Color { get; set; }
        public string Note { get; set; }

        public decimal Quantity { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal AmountTotal { get; set; }

        public string SaleOrderLineId { get; set; }
        public string LaboFinishLineId { get; set; }
        public string LaboBiteJointId { get; set; }
        public string LaboBridgeId { get; set; }


        [XmlArray("LaboOrderProductRel")]
        [XmlArrayItem("Record")]
        public List<LaboOrderProductRelXmlSampleDataRecord> LaboOrderProductRel { get; set; } = new List<LaboOrderProductRelXmlSampleDataRecord>();
        [XmlArray("LaboOrderToothRel")]
        [XmlArrayItem("Record")]
        public List<LaboOrderToothRelXmlSampleDataRecord> LaboOrderToothRel { get; set; } = new List<LaboOrderToothRelXmlSampleDataRecord>();
    }

    [XmlType("LaboOrderProductRel")]
    [XmlRoot("Record")]
    public class LaboOrderProductRelXmlSampleDataRecord
    {
        public string ProductId { get; set; }
    }

    [XmlType("LaboOrderToothRel")]
    [XmlRoot("Record")]
    public class LaboOrderToothRelXmlSampleDataRecord
    {
        public string ToothId { get; set; }
    }

    [XmlType("Record")]
    public class AppointmentXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string PartnerId { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        public int TimeHour { get; set; }
        public int TimeMinute { get; set; }
        public string Reason { get; set; }
        public int TimeExpected { get; set; }
        public bool IsRepeatCustomer { get; set; }
        public string DoctorId { get; set; }
        public string SaleOrderId { get; set; }
        public string State { get; set; }

        [XmlArray("AppointmentServices")]
        [XmlArrayItem("Record")]
        public List<ProductAppointmentRelXmlSampleDataRecord> AppointmentServices { get; set; } = new List<ProductAppointmentRelXmlSampleDataRecord>();
    }

    [XmlType("AppointmentServices")]
    [XmlRoot("Record")]
    public class ProductAppointmentRelXmlSampleDataRecord
    {
        public string ProductId { get; set; }
    }

    [XmlType("Record")]
    public class CustomerReceiptXmlSampleDataRecord
    {
        public string Id { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        public int WaitingTimeHour { get; set; }
        public int WaitingTimeMinute { get; set; }
        public int? ExaminationTimeHour { get; set; }
        public int? ExaminationTimeMinute { get; set; }
        public int? DoneTimeHour { get; set; }
        public int? DoneTimeMinute { get; set; }
        public int TimeExpected { get; set; }
        public string Note { get; set; }
        public string PartnerId { get; set; }
        public string DoctorId { get; set; }
        public string Reason { get; set; }
        public bool IsRepeatCustomer { get; set; }
        public bool IsNoTreatment { get; set; }
        public string State { get; set; }

        [XmlArray("CustomerReceiptProductRels")]
        [XmlArrayItem("Record")]
        public List<CustomerReceiptProductRelXmlSampleDataRecord> CustomerReceiptProductRels { get; set; } = new List<CustomerReceiptProductRelXmlSampleDataRecord>();
    }

    [XmlType("AppointmentServices")]
    [XmlRoot("Record")]
    public class CustomerReceiptProductRelXmlSampleDataRecord
    {
        public string ProductId { get; set; }
    }

    [XmlType("Record")]
    public class PhieuThuChiXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string LoaiThuChiId { get; set; }
        public string JournalId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public int DateRound { get; set; } // để set date.now - this
    }

    [XmlType("Record")]
    public class PurchaseOrderXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string PartnerId { get; set; }
        public string PickingTypeId { get; set; }
        public string JournalId { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        [XmlArray("OrderLines")]
        [XmlArrayItem("Record")]
        public List<PurchaseOrderLineXmlSampleDataRecord> OrderLines { get; set; } = new List<PurchaseOrderLineXmlSampleDataRecord>();
    }

    [XmlType("OrderLines")]
    [XmlRoot("Record")]
    public class PurchaseOrderLineXmlSampleDataRecord
    {
        public decimal ProductQty { get; set; }
        public string ProductUOMId { get; set; }
        public string ProductId { get; set; }
        public decimal PriceUnit { get; set; }
    }

    [XmlType("Record")]
    public class StockPickingXmlSampleDataRecord
    {
        public string Id { get; set; }
        public string PartnerId { get; set; }
        public string PickingTypeId { get; set; }
        public int DateRound { get; set; } // để set date.now - this
        [XmlArray("OrderLines")]
        [XmlArrayItem("Record")]
        public List<StockMoveXmlSampleDataRecord> MoveLines { get; set; } = new List<StockMoveXmlSampleDataRecord>();
    }

    [XmlType("OrderLines")]
    [XmlRoot("Record")]
    public class StockMoveXmlSampleDataRecord
    {
        public string Name { get; set; }
        public string PartnerId { get; set; }
        public decimal ProductUOMQty { get; set; }
        public decimal? ProductQty { get; set; }
        public string ProductUOMId { get; set; }
        public string ProductId { get; set; }
        public double? PriceUnit { get; set; }
        public int DateRound { get; set; } // để set date.now - this
    }

}

