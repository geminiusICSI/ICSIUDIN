using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICSI_UDIN.Models
{
    public class GenerateUDIN
    {
        [Required(ErrorMessage = "MRN Number must be required")]
        public string MRNNumber { get; set; }
        [Required(ErrorMessage = "Client Name must be required")]
        public string ClientName { get; set; }
        [Required(ErrorMessage = "CIN Number must be required")]
        public string CINNumber { get; set; }
        [Required(ErrorMessage = "Financial Year must be required")]
        public string FinancialYear { get; set; }
        [Required(ErrorMessage = "Date Of Signing Document must be required")]
        public string DateOfSigningDoc { get; set; }
        //[Required(ErrorMessage = "Document Description must be required")]
        public string DocDescription { get; set; }

        public string UDINNumber { get; set; }
        public int CertificateId { get; set; }
        public List<Certificate> lstCertificates { get; set; }
    }

    public class Certificate
    {
        public int CertificateId { get; set; }
        public string CertificateName { get; set; }
    }
}