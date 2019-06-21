/*
 Adhoc model class
 Author: Evgeny Chudaev
 Purpose: Represents the object of Adhoc ticket
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public class Adhoc : Requests
    {
        public int ID { get; set; }

        public string RequestorID { get; set; }

        [Required(ErrorMessage = "Please enter required by date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequiredByDate { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string SupportAnalyst { get; set; }

        public DateTime CompletedDate { get; set; }
        
        public string Details { get; set; }

        public string requestLocation { get; set; }

        public double latitute { get; set; }

        public double longtitude { get; set; }      

    }
}
