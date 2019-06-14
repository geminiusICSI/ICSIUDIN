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
            this._userRepository = new UserRepository(new UserDBEntities());
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

    }
}