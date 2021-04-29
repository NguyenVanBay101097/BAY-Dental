using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class ProcessUpdateNotification : INotification 
    {
        public readonly HttpContext _context;
        public ProcessUpdateNotification(HttpContext context)
        {
            _context = context;
        }
    }
}
