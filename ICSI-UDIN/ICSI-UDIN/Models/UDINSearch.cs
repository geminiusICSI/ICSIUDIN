using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ICSI_UDIN.Models
{
    public class UDINSearch
    {
        [Required]
        public string UDIN { get; set; }
        [Required]
        public string FinancialYear { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string FromDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public string ToDate { get; set; }
        public int UserId { get; set; }
    }
}