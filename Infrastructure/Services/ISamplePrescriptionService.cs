﻿using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISamplePrescriptionService : IBaseService<SamplePrescription>
    {
        Task<PagedResult2<SamplePrescriptionBasic>> GetPagedResultAsync(SamplePrescriptionPaged val);
        Task<SamplePrescription> GetPrescription(Guid id);
        Task<SamplePrescription> CreatePrescription(SamplePrescriptionSave val);
        Task UpdatePrescription(Guid id, SamplePrescriptionSave val);
        Task<IEnumerable<SamplePrescriptionBasic>> GetAutocomplete(SamplePrescriptionPaged val);
    }
}
