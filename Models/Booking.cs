using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace booking_mgmt.Models
{
    public class Booking
    {
        public int Booking_ID { get; set; }
        public int User_ID { get; set; }
        public int Cat_ID { get; set; }
        public int Movie_ID { get; set; }
        public int No_of_tickets { get; set; }
        public int Amount { get; set; }

        public string Cat_Type { get; set; }
        public string Movie_name { get; set; }
        public int Rate { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Movies { get; set; } = new List<SelectListItem>();

    }
}