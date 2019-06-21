/*
 Adhoc model class
 Author: Evgeny Chudaev
 Purpose: Represents the object of admin user (privileged user)
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public class Admins
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter login")]
        public string login { get; set; }

        [Required(ErrorMessage = "Please enter full name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter role")]
        public string role { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
