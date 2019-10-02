using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.EntityConfigurations
{
    public class ModelAccessReportConfiguration : IQueryTypeConfiguration<ModelAccessReport>
    {
        public void Configure(QueryTypeBuilder<ModelAccessReport> builder)
        {
            builder.ToView("model_access_report");
        }
    }
}
