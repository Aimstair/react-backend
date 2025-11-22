using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Dtos
{
    public class SuggestRecurringDto
    {
        public int BookingId { get; set; }
        public List<DateTime> RecurrenceDates { get; set; }
    }
}
