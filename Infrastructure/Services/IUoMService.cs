﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IUoMService: IBaseService<UoM>
    {
        Task<UoM> DefaultUOM();
    }
}
