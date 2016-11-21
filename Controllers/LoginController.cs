using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using aspLoginReg.Models;
using LoginApp.Factory;
using CryptoHelper;

namespace aspLoginReg.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginRepository loginFactory;

        public LoginController()
        {
            loginFactory = new LoginRepository();
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            return View("Login");
        }
        [HttpPost]
        [Route("registration")]
        public IActionResult Create(User newuser)
        {   
            if(ModelState.IsValid)
            {
                 loginFactory.Add(newuser);
                 ViewBag.User_Extracting = loginFactory.FindByID();
                 int current_id = ViewBag.User_Extracting.id;
                 HttpContext.Session.SetInt32("current_id", (int) current_id);
                 HttpContext.Session.SetString("display", "Successfully Registered");
                 return RedirectToAction("Dashboard");
            }
            List<string> temp_errors = new List<string>();
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            ViewBag.display = HttpContext.Session.GetString("display");
            ViewBag.User_all = loginFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("Success");
        }
        [HttpPost]
        [RouteAttribute("login")]
        public IActionResult Login(string email, string password)
        {
            List<string> temp_errors = new List<string>();
            if(email == null || password == null)
            {
                temp_errors.Add("Enter Email and Password Fields to Login");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Index");
            }
            User check_user = loginFactory.FindEmail(email);
            if(check_user == null)
            {
                temp_errors.Add("Email is not registered");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Index");
            }
            bool correct = Crypto.VerifyHashedPassword((string) check_user.password, password);
            if(correct)
            {
                HttpContext.Session.SetString("display", "Successfully Logged in!");
                HttpContext.Session.SetInt32("current_id", check_user.id);
                return RedirectToAction("Dashboard");
            }
            else{
                temp_errors.Add("Password is not matching");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [RouteAttribute("logout")]
         public IActionResult Logout()
         {
             HttpContext.Session.Clear();
             return RedirectToAction("Index");

         }
    }
}
