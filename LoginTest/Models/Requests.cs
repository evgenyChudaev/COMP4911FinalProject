/*
 Requests interface
 Author: Evgeny Chudaev
 Purpose: Requests interface is implemented by all 3 types of requests so that requests can be added to a single collections if needed
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public interface Requests
    {
       DateTime SubmittedDate { get; set; }
       string SupportAnalyst { get; set; }
       DateTime CompletedDate { get; set; }
       string RequestorID { get; set; }
    }
}
