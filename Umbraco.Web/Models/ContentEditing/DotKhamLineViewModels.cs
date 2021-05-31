using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class DotKhamLinePaged
    {
        public DotKhamLinePaged()
        {
            Limit = 20;
        }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
    public class DotKhamLineBasic
    {
        public Guid Id { get; set; }

        public DateTime DotKhamDate { get; set; }

        public string DotKhamName { get; set; }
        public SaleOrderSimple DotKhamSaleOrder { get; set; }
        public PartnerSimple DotKhamPartner { get; set; }

        public string ProductName { get; set; }

        public string DotKhamDoctorName { get; set; }

        public string NameStep { get; set; }
        public string Note { get; set; }
        public IEnumerable<ToothBasic> Teeth { get; set; } = new List<ToothBasic>();
    }

    public class ProductDotKhamLineSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class DotKhamLineDisplay
    {
        public Guid Id { get; set; }

        public string NameStep { get; set; }

        public ProductSimple Product { get; set; }

        public Guid? SaleOrderLineId { get; set; }

        public string Note { get; set; }

        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();
    }


    public class DotKhamLineSaveVM
    {
        public Guid Id { get; set; }

        public string NameStep { get; set; }

        //Chi su dung cho nhung line Id = Guid.Emplty
        public Guid? SaleOrderLineId { get; set; }

        //dung de tao
        public Guid? ProductId { get; set; }

        //dung de update
        public string Note { get; set; }

        //dung de update
        public IEnumerable<Guid> ToothIds { get; set; } = new List<Guid>();
    }

    public class DotKhamLineChangeRouting
    {
        public Guid Id { get; set; }

        public Guid RoutingId { get; set; }
    }
}
