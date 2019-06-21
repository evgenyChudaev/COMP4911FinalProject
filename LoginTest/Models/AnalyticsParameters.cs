/*
 Analytics parameters model class
 Author: Evgeny Chudaev
 Purpose: Represents the object of a set of parameters for analytics filters
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public class AnalyticsParameters
    {
        public string yearSubmitted { get; set; }        
        public string locationSubmitted { get; set; }
        public string requestTypeSubmitted { get; set; }
        public string requestStatus { get; set; }
        public string requestOverdue { get; set; }
    }
}
