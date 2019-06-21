/*
 ErrorView model class
 Author: Evgeny Chudaev
 Purpose: Represents the object of an error related to rounting/views
 */

using System;

namespace LoginTest.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}