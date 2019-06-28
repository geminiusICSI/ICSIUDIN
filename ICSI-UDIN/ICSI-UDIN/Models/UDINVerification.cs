using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ICSI_UDIN.Models
{
    public class UDINVerification
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string MembershipNumber { get; set; }
        [Required]
        public string CaptchaCode { get; set; }
    }
}