using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ProductStepSimple
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductStepDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Tham chiếu Product.type Service
        /// </summary>
        public Guid? ProductId { get; set; }
        public ProductDisplay Product { get; set; }
        
        /// <summary>
        /// Mặc định 
        /// </summary>
        public bool Default { get; set; }
    }

    //public class ProductStepPaged
    //{
    //    public ProductStepPaged()
    //    {
    //        Limit = 20;
    //    }
    //    public int Offset { get; set; }
    //    public int Limit { get; set; }
    //    public string Search { get; set; }
    //    public Guid? ProductId { get; set; }
    //    public Product Product { get; set; }
    //}
}
