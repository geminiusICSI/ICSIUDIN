﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ICSI_UDIN.Models
{
    public class MemberRegistration
    {


        [Required(ErrorMessage = "Membership Number must be required")]
        [Display(Name = "Membership Number :")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Membership Number must be numeric")]
        public int MRN { get; set; }

        [Required(ErrorMessage = "Date of Birth must be required")]
        [Display(Name = "Date Of Birth")]
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DOB { get; set; }

    } 

    }
