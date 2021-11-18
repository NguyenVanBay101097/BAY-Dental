﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResInsurancePaymentService : IBaseService<ResInsurancePayment>
    {
        Task<ResInsurancePayment> CreateResInsurancePayment(ResInsurancePaymentSave val);

        Task ActionPayment(Guid id);
    }
}
