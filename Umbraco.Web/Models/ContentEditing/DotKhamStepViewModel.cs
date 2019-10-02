using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class DotKhamStepSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class DotKhamStepDisplay
    {
        public DotKhamStepDisplay()
        {
            State = "draft";
        }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public int? Order { get; set; }

        public bool IsInclude { get; set; }

        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public Guid? DotKhamId { get; set; }
        public DotKhamSimple DotKham { get; set; }

        public Guid? InvoicesId { get; set; }
        public AccountInvoiceCbx Invoice { get; set; }

        public bool IsDone { get; set; }
    }
}
