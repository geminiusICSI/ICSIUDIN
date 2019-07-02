using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ICSI_UDIN.Models
{
    public class UDINSearch
    {
        
        public string UDIN { get; set; }
        
        public string FinancialYear { get; set; }
        
        
        public string FromDate { get; set; }
        
        
        public string ToDate { get; set; }
        public int UserId { get; set; }
    }
}