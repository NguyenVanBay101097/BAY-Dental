using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductAppointmentRel
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid AppoinmentId { get; set; }
        public Appointment Appointment { get; set; }
    }
}
