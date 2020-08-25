﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HrPayslipProfile : Profile
    {
        public HrPayslipProfile()
        {
            CreateMap<HrPayslip, HrPayslipDisplay>();
            CreateMap<HrPayslipDisplay, HrPayslip>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<HrPayslipSave, HrPayslip>().ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
