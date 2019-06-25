using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICSI_UDIN.Models;
using ICSI_UDIN.Repository;

namespace ICSI_UDIN.Controllers
{
    public class HomeController : Controller
    {

        private IUserRepository _userRepository;
        public HomeController()
        {
            this._userRepository = new UserRepository(new ICSI_DBModleEntities());
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(tblUser obj)
        {
            bool check = false;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                check = _userRepository.CheckLogin(obj);
                if (check == false)
                {
                    message = "Invalid login credentials";
                    ViewBag.Message = message;
                }
                else
                {
                    return RedirectToAction("MemberRegistration");
                }
            }
            else
            {
                ModelState.AddModelError("", " Wrong User/Password.");
            }
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
        public ActionResult UDINVerification(RP_UDINVerification_Result obj)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UDINCaptchaCode"])) && obj.CaptchaCode == Convert.ToString(Session["UdinCaptchaCode"]))
            {
                var result = _userRepository.GetUDINVerification(obj);

                return RedirectToAction("UDINDocumentDetails");

            }
            return View();


        }
        [HttpGet]
        public ActionResult UDINDocumentDetails()
        {
            return View();
        }

        public ActionResult MembershipRegistation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MembershipRegistation(MemberRegistration objMemberRegistration, string premember)
        {
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
                    objtblUser.DOB = Convert.ToDateTime(DateOfBirth);
                    objtblUser.MobileNumber = null;
                    objtblUser.EmailId = EmailId;
                    objtblUser.CreatedDate = DateTime.Now;
                    _userRepository.InsertUser(objtblUser);
                    #endregion

                    #region Insert in tblUDIN
                    tblUDIN objtblUDIN = new tblUDIN();
                    objtblUDIN.MembershipNumber = MembershipNo;
                    objtblUDIN.YearOfEnrollment = null;
                    objtblUDIN.CreatedDate = DateTime.Now;
                    objtblUDIN.CreatedBy = null;
                    objtblUDIN.ModifyDate = null;
                    objtblUDIN.UDINUniqueCode = null;
                    objtblUDIN.IsValid = "1";
                    objtblUDIN.DocumentTypeId = 0;
                    objtblUDIN.DocumentDescription = null;
                    objtblUDIN.StatusId = 0;
                    objtblUDIN.UserId = objtblUser.UserId;
                    _userRepository.InserttblUDINUser(objtblUDIN);
                    #endregion

                    TempData["UserId"] = objtblUser.UserId;
                    return RedirectToAction("CreatePassword");
                }
                else
                    ViewBag.ErrorMsg = "Certificate of Practical should be greater than zero.";
            }
            return View();
        }


        public ActionResult SearchUDIN()
        {
            return View();
        }


        public ActionResult ForgetPassword()
        {
            return View();
        }


        public ActionResult HelpDeskFacilty()
        {
            return View();
        }

        public ActionResult CreatePassword()
        {
            _userRepository.UDINGeneration("F5922");

            return View();
        }

        [HttpPost]
        public ActionResult CreatePassword(CreatePassword objCreatePassword)
        {
            if (ModelState.IsValid)
            {
                int userId = Convert.ToInt32(TempData["UserId"]);

            }
            return View(objCreatePassword);
        }

    }
}