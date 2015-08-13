using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiTester.Models
{
    public struct CertificateInfo
    {
        public string SubjectName { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public bool FoundMerchantId { get; set; }
        public bool FoundPrivateKeyParameters { get; set; }
        public string ClientIpAddress { get; set; }
    }
}