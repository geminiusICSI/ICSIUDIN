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
        [AllowAnonymous]
        public ActionResult Index(string username ,string password)
        {
            bool check = false;
            string message = string.Empty;
            check = _userRepository.CheckLogin(username, username);
            if(check ==true)
            {
            }
            else
            {
                message = "Username and/or password is incorrect.";
            }
           ViewBag.Message = message;
            return View(message);
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

        public ActionResult Create(FormCollection collection, tblUser  objuser)

        {
            try
            {
                var User = new tblUser();
                User.ID = 0;
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
        public ActionResult UDINVerification(tblUDIN obj)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["UDINCaptchaCode"])) && obj.Lname == Convert.ToString(Session["UdinCaptchaCode"]))
            {
               var result= _userRepository.GetUDINVerification(obj);
             
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



    }
}