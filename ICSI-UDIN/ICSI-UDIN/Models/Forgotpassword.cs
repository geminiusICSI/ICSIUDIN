using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ICSI_UDIN.Models
{
    public class Forgotpassword
    {
        [Required]
        [Display(Name = "Membership Number :")]
        public string MemmbershipNumber { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }


        [Required]
        [Display(Name = "Year Of Enrolment :")]
        [RegularExpression(@"^([0-9]{4})$", ErrorMessage = " Year Of Enrolment  must be numeric")]
        public int YearOfEnrolment { get; set; }

        public  string MaskEmail(string s)
        {
             string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{4}@)";
             string result = Regex.Replace(s, pattern, m => new string('*', m.Length));
              return result;
        }
        public string EmailId { set; get; }

    }
}