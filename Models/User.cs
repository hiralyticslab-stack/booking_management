using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace booking_mgmt.Models
{
    public class User
    {
        public int User_ID { get; set; }
        public string User_Name { get; set; }
        public string Email_ID { get; set; }
        public string User_Password { get; set; }
        public string City { get; set; }
        public string PhoneNo { get; set; }

    }
}