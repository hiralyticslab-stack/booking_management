using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace booking_mgmt.Models
{
    public class Movie
    {
        public int Movie_ID { get; set; }
        public string Movie_name { get; set; }
        //[DisplayFormat(DataFormatString ="{0:dd-MM-yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Release_Date { get; set; }
        public int Cat_ID { get; set; }
        public IEnumerable<SelectListItem> category { get; set; }

        public int Rate { get;set; }
    }
}