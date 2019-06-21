/*
 Incident model class
 Author: Evgeny Chudaev
 Purpose: Represents the object of Incident ticket
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public class Incident : Requests
    {
        public int ID { get; set; }

        public string RequestorID { get; set; }      

        public DateTime SubmittedDate { get; set; }

        public string Severity { get; set; }

        public string Status { get; set; }

        public string SupportAnalyst { get; set; }

        public DateTime CompletedDate { get; set; }
        
        public string TicketDetails { get; set; }

        public string requestLocation { get; set; }

        public double latitute { get; set; }

        public double longtitude { get; set; }       

    }
}
