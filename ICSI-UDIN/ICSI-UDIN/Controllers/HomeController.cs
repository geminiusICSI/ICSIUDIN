using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ICSI_UDIN.Models;
using ICSI_UDIN.Repository;

namespace ICSI_UDIN.Controllers
{
    public class HomeController : Controller
    {

        private IUserRepository _userRepository;
        public HomeController()
        {
            this._userRepository = new UserRepository(new ICSI_DBModelEntities());
        }

        [HttpGet]
        public ActionResult Index()
        {
            GetTotalUserUDIN_Result totalUsers = _userRepository.GetTotalUDINUser();
            if (totalUsers != null)
            {
                Session["TotalUDINs"] = totalUsers.TotalUDINs;
                Session["ToTalUsers"] = totalUsers.TotalUsers;
            }
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(ICSI_UDIN.Models.Login obj)
        //{
        //    bool check = false;
        //    //tblUser objUser = new tblUser();
        //    string message = string.Empty;
        //    if (ModelState.IsValid)
        //    {
        //        tblUser objtbluser = new tblUser();
        //        objtbluser.UserName = obj.UserName;
        //        objtbluser.Password = obj.Password;

        //        check = _userRepository.CheckLogin(objtbluser);
        //        if (check == false)
        //        {
        //            message = "Invalid login credentials";
        //            ViewBag.Message = message;
        //        }
        //        else
        //        {
        //            return RedirectToAction("MembershipRegistation");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", " Wrong User/Password.");
        //    }
        //    return View(obj);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ICSI_UDIN.Models.Login obj)
        {
            bool check = false;
            bool checkUdn = false;

            tblUser objtbluser = new tblUser();
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                int userid = 0;
                tblUser tblUser=_userRepository.GetUserByUserName(obj.UserName);
                if (tblUser == null)
                {
                    ViewBag.Message = "You are not registered. Please go to Member registeration link.";
                    return View(obj);
                }
                else
                {
                    userid = tblUser.UserId;
                }
                objtbluser.UserName = obj.UserName;
                objtbluser.UserId = userid;
                Session["LogedUserID"] = objtbluser.UserName.ToString();
                Session["UserID"] = userid;
                objtbluser.Password = obj.Password;

                //for count user
                Session["UserName"] = objtbluser.UserName;
               
               

                check = _userRepository.CheckLogin(objtbluser);
                if (check == false)
                {
                    message = "Invalid login credentials";
                    ViewBag.Message = message;
                }
                else
                {
                    checkUdn = _userRepository.CheckUdn(objtbluser);
                    if (checkUdn == false)
                    {
                        return RedirectToAction("GenerateUDIN");
                    }
                    else
                    {
                        return RedirectToAction("UDINList");
                    }


                }
            }
            else
            {
                ModelState.AddModelError("", " Wrong User/Password.");
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult ForgotPassword(Forgotpassword objpassword)
        {
            tblUser objuser = new tblUser();
            string msg = string.Empty;
            if (ModelState.IsValid)
            {
                string MemmbershipNumber = Convert.ToString(objpassword.MemmbershipNumber);
                DateTime DOB = Convert.ToDateTime(objpassword.DOB);
                int YearOfEnrollment = Convert.ToInt32(objpassword.YearOfEnrolment);
                List<Forgotpassword> EmailId = _userRepository.FogotPassword(MemmbershipNumber, DOB, YearOfEnrollment);
                if (EmailId.Count == 0)
                {
                    msg = "Invalid MemmbershipNumber/DOB/YearOfEnrollment";
                    ViewBag.msg = msg;
                }
                else
                {
                    int year = Convert.ToInt32(DOB.Year);
                    string Email = EmailId[0].EmailId.ToString();
                    string maskedEmail = objpassword.MaskEmail(Email);
                    TempData["mail"] = maskedEmail;
                    TempData.Keep();
                    string NewPassword = year + MemmbershipNumber;
                    objuser.EmailId = Email;
                    objuser.Password = NewPassword;
                    _userRepository.UpdatePassword(objuser);
                    string Body = "Please use this Password :" + NewPassword + ". Please keep this for future communications.";
                    _userRepository.sendMail(Email, "Forgot Password", Body);
                    return RedirectToAction("NewPassword");

                }

            }
            else
            {
                ModelState.AddModelError("", " Wrong MemmbershipNumber/DOB/EmailId.");
            }

            return View();
        }
        public ActionResult NewPassword(string maskedEmail)
        {
            maskedEmail = TempData.Peek("mail").ToString();
            string msg = "New Generate Password has sent your maild please check : " + maskedEmail;
            ViewBag.msg = msg;

            return View();
        }

        public ActionResult Details(int UserId)

        {
            var objuser = _userRepository.GetUserByID(UserId);
            var User = new tblUser();
            User.UserName = objuser.UserName;
            User.Password = objuser.Password;
            return View(User);
        }
        [HttpPost]

        public ActionResult Create(FormCollection collection, tblUser objuser)

        {
            try
            {
                var User = new tblUser();
                User.UserId = 0;
                User.UserName = objuser.UserName;
                User.Password = objuser.Password;
                _userRepository.InsertUser(User); // Passing data to InsertEmployee of UserRepository

                return RedirectToAction("Index");

            }
            catch
            {
                return View();

            }

        }

        [HttpPost]

        public ActionResult Edit(FormCollection collection, tblUser objuser)

        {

            try

            {

                var Employee = new tblUser();

                _userRepository.UpdateUser(objuser); // calling UpdateUser method of UserRepository

                return RedirectToAction("Index");

            }

            catch

            {

                return View();

            }
        }

        [HttpPost]
        public ActionResult GenerateAlphaNumericOTP()
        {
            string numbers = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random objrandom = new Random();
            string passwordString = "";
            string strrandom = string.Empty;
            for (int i = 0; i < 10; i++)
            {
                int temp = objrandom.Next(0, numbers.Length);
                passwordString = numbers.ToCharArray()[temp].ToString();
                strrandom += passwordString;
            }
            ViewBag.anotp = strrandom;
            return View("Index");
        }

        public ViewResult Login()
        {

            return View();

        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        // GET: UDIN
        [HttpGet]
        public ActionResult UDINVerification()
        {

            return View();
        }

        public ActionResult GenerateCaptcha(int? New)
        {
            string str = Convert.ToString(Session["ID"]);

            UDIN_Captcha UdinCph = new UDIN_Captcha();
            byte[] imageByteData = UdinCph.CreateCaptchaImage(New);
            return File(imageByteData, "~/images/ImgCaptcha");

        }


        [HttpPost]
        public ActionResult UDINVerification(UDINVerification obj)
        {

            if (!string.IsNullOrEmpty(Convert.ToString(Session["UDINCaptchaCode"])) && obj.CaptchaCode == Convert.ToString(Session["UdinCaptchaCode"]))
            {
                ViewBag.CaptchMessage = "";
                var result = _userRepository.GetUDINVerification(obj);
                if (result != null)
                {
                    ViewBag.VNF = "";
                    TempData["Data"] = result;
                    return RedirectToAction("UDINDocumentDetails", obj);
                }
                else
                {
                    ViewBag.VNF = "UDIN does not exist.";
                }


            }
            else
            {
                ViewBag.CaptchMessage = "Captcha is not Match";
            }

            return View();


        }
        [HttpGet]
        public ActionResult UDINDocumentDetails()
        {
            RP_UDINVerification_Result obj = null;
            if (TempData["Data"] != null)
            {
                obj = TempData["Data"] as RP_UDINVerification_Result;

            }

            return View(obj);
        }

        public ActionResult MembershipRegistation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MembershipRegistation(MemberRegistration objMemberRegistration, string premember)
        {
            ViewBag.Path = ConfigurationManager.AppSettings["Path"].ToString() + "Home/CreatePassword";
            if (ModelState.IsValid)
            {
                string prememberval = string.Empty;
                prememberval = premember == "1" ? "A" : "F";

                ICSI_SoapService.Service obj = new ICSI_SoapService.Service();
                var soapData = obj.GetMemberShipData(prememberval, objMemberRegistration.MRN);
                int CertificateofPracticalNumber = Convert.ToInt32(soapData.CertificateofPracticalNumber);
                if (CertificateofPracticalNumber > 0)
                {
                    string MembershipNo = prememberval + soapData.MembershipNo;
                    string DateOfBirth = soapData.DateofBirth;
                    string FirstName = soapData.FirstName;
                    string MiddleName = soapData.MiddleName;
                    string LastName = soapData.LastName;
                    string PreMembNo = soapData.premembno;
                    string BarredMember = soapData.BarredMember;
                    string BarredDate = soapData.BarredDate;
                    string EnrollDate = soapData.EnrollDate;
                    string CPDate = soapData.CPDate;
                    string Name = soapData.Name;
                    string EmailId = soapData.EmailID;
                    string MemberStatus = soapData.MemberStatus;
                    string Msg = soapData.MSg;

                    #region Insert in tblUser
                    tblUser objtblUser = new tblUser();
                    objtblUser.UserName = MembershipNo;
                    objtblUser.Password = null;
                    objtblUser.FirstName = FirstName;
                    objtblUser.MiddleName = MiddleName;
                    objtblUser.LastName = LastName;

                    objtblUser.DOB = DateTime.ParseExact(DateOfBirth, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat);
                    objtblUser.MobileNumber = null;
                    objtblUser.EmailId = EmailId;
                    objtblUser.CreatedDate = DateTime.Now;
                    //_userRepository.InsertUser(objtblUser);
                    int status = _userRepository.InsertTblUser(objtblUser);
                    #endregion

                    if (status > 0)
                    {
                        #region Insert in tblUDIN
                        //tblUDIN objtblUDIN = new tblUDIN();
                        //objtblUDIN.MembershipNumber = MembershipNo;
                        //objtblUDIN.YearOfEnrollment = null;
                        //objtblUDIN.CreatedDate = DateTime.Now;
                        //objtblUDIN.CreatedBy = null;
                        //objtblUDIN.ModifyDate = null;
                        //objtblUDIN.UDINUniqueCode = null;
                        //objtblUDIN.IsValid = "1";
                        //objtblUDIN.DocumentTypeId = 0;
                        //objtblUDIN.DocumentDescription = null;
                        //objtblUDIN.StatusId = 0;
                        //objtblUDIN.UserId = objtblUser.UserId;
                        //_userRepository.InserttblUDINUser(objtblUDIN);
                        #endregion

                        TempData["UserId"] = objtblUser.UserId;
                        ViewBag.ErrorMsg = "1";
                        //return RedirectToAction("CreatePassword");
                    }
                    else
                        return RedirectToAction("Index");

                }
                else
                    ViewBag.ErrorMsg = "Holding Certificate of Practice";
            }
            return View();
        }


        
        public ActionResult SearchUDIN()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult SearchUDIN(UDINSearch obj)
        //{

        //    if (Session["UserId"] != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            obj.UserId = Convert.ToInt32(Session["UserId"]);
        //            var result = _userRepository.GetUDINList(obj);
        //            if (result != null && result.Count > 0)
        //            {
        //                TempData["UDINList"] = result;
        //                return RedirectToAction("UDINList");

        //            }
        //            ViewBag.Mesaage = "No Data Found";
        //            return View(obj);

        //        }
        //    }
        //    else
        //    {
        //        ViewBag.Session = "You are not Authorized User.";
        //    }


        //    return View(obj);


        //}

        //[HttpGet]
        //public ActionResult UDINList()
        //{
        //    var result = TempData.Peek("UDINList") as List<RP_GetUDINList_Result>;
        //    if (result != null)
        //    {
        //        return View(result);
        //    }
        //    return View("SearchUDIN");

        //}

        [HttpPost]
        public ActionResult SearchUDIN(UDINSearch obj)
        {

            if (Session["UserId"] != null)
            {
                if (ModelState.IsValid)
                {
                    TempData["SearchUDIN"] = 1;
                    obj.UserId = Convert.ToInt32(Session["UserId"]);
                    var result = _userRepository.GetUDINList(obj);
                    if (result != null && result.Count > 0)
                    {
                        TempData["UDINList"] = result;
                        return RedirectToAction("UDINList");

                    }
                    ViewBag.Mesaage = "No Data Found";
                    return View(obj);

                }
            }
            else
            {
                ViewBag.Session = "You are not Authorized User.";
            }


            return View(obj);


        }

        [HttpGet]
        public ActionResult UDINList()
        {
            ViewBag.Message = "";
            if (Convert.ToString(TempData["SearchUDIN"]) == "1")
            {
                var result = TempData.Peek("UDINList") as List<RP_GetUDINList_Result>;
                if (result != null)
                {
                    return View(result);
                }
            }
            else
            {
                UDINSearch obj = new UDINSearch();
                obj.UserId = Convert.ToInt32(Session["UserId"]);
                var result = _userRepository.GetUDINList(obj);
                if (result != null && result.Count > 0)
                {
                    TempData["UDINList"] = result;
                    return View(result);

                }
            }
            ViewBag.Message = "No Data Found";
            return View();

        }

        [HttpGet]
        public ActionResult CancelUDIN(RP_GetUDINList_Result obj)
        {
            TempData["Revoke"] = obj;
            return View(obj);
        }

        [HttpPost]
        public ActionResult CancelUDIN(string UDINRevokeReason)
        {

            RP_GetUDINList_Result obj = TempData.Peek("Revoke") as RP_GetUDINList_Result;
            obj.UDINRevokeReason = UDINRevokeReason;
            if (obj.UDINRevokeReason == null || obj.UDINRevokeReason == "")
            {
                ViewBag.Message = "Please fill the reason";
                return View(obj);
            }
            int result = _userRepository.RevokeUDIN(obj);
            ViewBag.Message = "UDIN Revoke Successfully";
            return View("SearchUDIN");
        }



        [HttpGet]
        public ActionResult ExportToExcel()
        {
            var resultTempData = TempData.Peek("UDINList") as List<RP_GetUDINList_Result>;

            var gv = new GridView();
            gv.DataSource = resultTempData;
            gv.DataBind();
            gv.HeaderRow.Cells[0].Text = "UDIN Number";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("UDINList", resultTempData);
        }

        public ActionResult HelpDeskFacilty()
        {
            return View();
        }

        public ActionResult CreatePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePassword(CreatePassword objCreatePassword)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Path = ConfigurationManager.AppSettings["Path"].ToString() + "Home/Index";
                int userId = Convert.ToInt32(TempData["UserId"]);
                //userId = 4;
                tblUser objtblUser = new tblUser();
                objtblUser.UserId = userId;
                objtblUser.Password = objCreatePassword.ConfirmPassword;
                int status = _userRepository.updatetblUserById(objtblUser);
                if (status >= 0)
                {
                    ViewBag.ErrorMsg = 1;
                }
                
                
            }
            return View(objCreatePassword);
        }


        public ActionResult GenerateUDIN()
        {
            int UserId = Convert.ToInt32(Session["UserID"]);
            tblUser objtblUser = _userRepository.GetUserByID(UserId);
            GenerateUDIN objGenerateUDIN = new GenerateUDIN();
            objGenerateUDIN.MRNNumber = objtblUser.UserName;
            objGenerateUDIN.UDINNumber = _userRepository.UDINGeneration(objGenerateUDIN.MRNNumber);
            objGenerateUDIN.FinancialYear = DateTime.Now.Year + "-" + DateTime.Now.AddYears(1).Year.ToString().Substring(2, 2);
            //objGenerateUDIN.DateOfSigningDoc = DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/");

            objGenerateUDIN.lstCertificates = _userRepository.CertificateList(1);

            return View(objGenerateUDIN);
        }

        [HttpPost]
        public ActionResult GenerateUDIN(GenerateUDIN objGenerateUDIN, string rdbgroup, string rdbUDINgroup)
        {
            if (Convert.ToInt32(rdbgroup) == 1 || Convert.ToInt32(rdbgroup) == 2)
                objGenerateUDIN.lstCertificates = _userRepository.CertificateList(Convert.ToInt32(rdbgroup));
            else if (Convert.ToInt32(rdbgroup) == 3)
                objGenerateUDIN.lstCertificates = _userRepository.CertificateList(1);

            if (rdbgroup == "3" && string.IsNullOrEmpty(objGenerateUDIN.DocDescription))
            {
                ViewBag.Message = "Document description must be required";
                return View(objGenerateUDIN);
            }
            else if ((rdbgroup == "1" || rdbgroup == "2") && objGenerateUDIN.CertificateId == 0)
            {
                if (rdbgroup == "1")
                    ViewBag.Message = "Please choose Certificate";
                else
                    ViewBag.Message = "Please choose Report";

                return View(objGenerateUDIN);
            }

            if (ModelState.IsValid)
            {
                int UserId = Convert.ToInt32(Session["UserID"]);
                bool flag = _userRepository.CheckUdnExistance(objGenerateUDIN.UDINNumber);

                if (flag == false)
                {
                    #region Insert in tblUDIN
                    tblUDIN objtblUDIN = new tblUDIN();
                    objtblUDIN.MembershipNumber = objGenerateUDIN.MRNNumber;
                    objtblUDIN.YearOfEnrollment = null;
                    objtblUDIN.CreatedDate = DateTime.Now;
                    objtblUDIN.CreatedBy = null;
                    objtblUDIN.ModifyDate = null;
                    objtblUDIN.UDINUniqueCode = objGenerateUDIN.UDINNumber;
                    objtblUDIN.IsValid = "Y";
                    if (string.IsNullOrEmpty(objGenerateUDIN.DocDescription))
                        objtblUDIN.DocumentTypeId = objGenerateUDIN.CertificateId;
                    else
                    {
                        objtblUDIN.DocumentDescription = objGenerateUDIN.DocDescription;
                        objtblUDIN.DocumentTypeId = 13;
                    }
                    objtblUDIN.CertificateTypeId = Convert.ToInt32(rdbgroup);
                    objtblUDIN.StatusId = 0;
                    objtblUDIN.UserId = UserId;
                    objtblUDIN.FinancialYear = objGenerateUDIN.FinancialYear;
                    objtblUDIN.ClientName = objGenerateUDIN.ClientName;
                    objtblUDIN.CINNumber = objGenerateUDIN.CINNumber;
                    objtblUDIN.UDINInitiative = Convert.ToInt32(rdbUDINgroup);
                    objtblUDIN.DateOfSigningDoc = Convert.ToDateTime(objGenerateUDIN.DateOfSigningDoc);

                    int status = _userRepository.InserttblUDINUser(objtblUDIN);

                    if (status > 0)
                    {
                        string EmailTo = _userRepository.GetUserByID(UserId).EmailId;
                        //string Body = "Thank you for registering UDIN. Your 16 digit UDIN number is " + objGenerateUDIN.UDINNumber + ". Please keep this for future communications.";
                        // EmailTo = "akumar@gemini-us.com";
                        string Body = _userRepository.UDINGenerationEmailBody(objGenerateUDIN.MRNNumber, objGenerateUDIN.UDINNumber, objGenerateUDIN.CINNumber, objGenerateUDIN.FinancialYear, objtblUDIN.ID, objGenerateUDIN.DateOfSigningDoc);

                        if (!string.IsNullOrEmpty(EmailTo))
                            _userRepository.sendMail(EmailTo, "UDIN generation", Body);

                        _userRepository.InsertGenerateUDIN();
                        ViewBag.Message = "UDIN has been generated successfully. UDIN number is " + objGenerateUDIN.UDINNumber + ". Kindly note that the Certifiate can only be revoked with in 15 days of issue.";
                    }
                    #endregion
                }
                else
                {
                    ViewBag.Message = "Please refresh the page for multiple UDIN generation.";
                }
            }
            return View(objGenerateUDIN);
        }


        public JsonResult CertificateList(int TypeOfDocument)
        {
            List<Certificate> lstcertificates = new List<Certificate>();
            lstcertificates = _userRepository.CertificateList(Convert.ToInt32(TypeOfDocument));

            return Json(lstcertificates);
        }


        public FileResult download(string Parameter)
        {


           
            string filename = Server.MapPath("~/PDF/UdinGuidelines.pdf");

            string contentType = "application/pdf";
            //Parameters to file are
            //1. The File Path on the File Server
            //2. The content type MIME type
            //3. The parameter for the file save by the browser
            return File(filename, contentType, "UdinGuidelines.pdf");
        }





    }

}