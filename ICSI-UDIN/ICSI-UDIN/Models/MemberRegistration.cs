using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ICSI_UDIN.Models
{
    public class MemberRegistration
    {


        [Required]
        [Display(Name = "Membership Number :")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "Membership Number must be numeric")]
        public int MRN { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }


        [Required]
        [Display(Name = "Year Of Enrolment :")]
        [RegularExpression(@"^([0-9]{4})$", ErrorMessage = " Year Of Enrolment  must be numeric")]
        public int YearOfEnrolment { get; set; }

    } 

    }
