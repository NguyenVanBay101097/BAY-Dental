using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductCategoryImportExcel
    {
        public string Name { get; set; }
    }

    public class ProductCategoryImportExcelBaseViewModel
    {
        public string FileBase64 { get; set; }

        public string Type { get; set; }
    }

    public class ProductCategoryServiceImportExcelRow
    {
        public string Name { get; set; }
    }
    public class ProductCategoryMedicineImportExcelRow
    {
        public string Name { get; set; }
    }
}
