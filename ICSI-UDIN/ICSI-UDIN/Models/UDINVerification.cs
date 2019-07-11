using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICSI_UDIN.Models
{
    public class UDINVerification
    {
      
        public string FName { get; set; }
     
        public string EmailId { get; set; }
      
        public string MobileNumber { get; set; }
        [Required]
        public string MembershipNumber { get; set; }
        [Required]
        public string CaptchaCode { get; set; }
    }
}