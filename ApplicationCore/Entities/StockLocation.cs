using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockLocation: BaseEntity
    {
        public StockLocation()
        {
            Active = true;
        }

        public string Usage { get; set; }

        public string Name { get; set; }

        public Guid? ParentLocationId { get; set; }

        public StockLocation ParentLocation { get; set; }

        public ICollection<StockLocation> Childs { get; set; } = new List<StockLocation>();

        public string CompleteName { get; set; }

        public bool Active { get; set; }

        public bool ScrapLocation { get; set; }

        public int? ParentLeft { get; set; }

        public int? ParentRight { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public string NameGet { get; set; }
    }
}
