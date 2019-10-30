using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductLaboSave
    {
        [Required]
        public string Name { get; set; }
        public decimal? PurchasePrice { get; set; }
    }
}
