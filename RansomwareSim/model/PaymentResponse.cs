using System;
using System.Collections.Generic;
using System.Text;

namespace RansomwareSim.model
{
    internal class PaymentResponse
    {
        public string AuthorizationUrl { get; set; }
        public string Reference { get; set; }
    }
}

