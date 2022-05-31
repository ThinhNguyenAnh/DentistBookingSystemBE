﻿using DentisBooking.Data.Entities;
using DentisBooking.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentistBooking.ViewModels.System.Bookings
{
    public class BookingRequest
    {
        public List<Guid> DentistIds { get; set; }
        public List<int> ServiceIds { get; set; }
        public List<KeyTime> KeyTimes { get; set; }
        public Guid UserId { get; set; }
        public Decimal Total { get; set; }
        public string? Note { get; set; }
    }
}
