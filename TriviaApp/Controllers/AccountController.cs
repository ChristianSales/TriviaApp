using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TriviaApp.Models;

namespace TriviaApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(User UserModel)
        {
            using (TriviaDBEntities db = new TriviaDBEntities())
            {
                var userInfo = db.Users.Where(x => x.UserName == UserModel.UserName && x.Password == UserModel.Password).FirstOrDefault();
                if (userInfo == null)
                {
                    UserModel.LoginErrorMessage = "Wrong username or password.";
                    return View("index", UserModel);
                }
                else
                {
                   
                    Session["userID"] = userInfo.UserID;
                    Session["userName"] = userInfo.UserName;
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult LogOut()
        {
            int userId = (int)Session["UserID"];
            Session.Abandon();
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        public ActionResult Register()
        {
            User UserModel = new User();
            return View(UserModel);
        }

        [HttpPost]
        public ActionResult Register(User UserModel)
        {
            using (TriviaDBEntities dbModel = new TriviaDBEntities())
            {
                if(dbModel.Users.Any(x => x.UserName == UserModel.UserName))
                {
                    ViewBag.DuplicateName = "Username is taken";
                    return View("Register", UserModel);
                }
                if (dbModel.Users.Any(x => x.Password == UserModel.Password))
                {
                    ViewBag.DuplicatePassword = "Password is taken";
                    return View("Register", UserModel);
                }
                UserModel.Stat = new Stat
                {
                    AmountCorrect = 0,
                    AmountWrong = 0,
                    QuizTaken = 0,
                };
                dbModel.Users.Add(UserModel);
                dbModel.SaveChanges();

            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "Registration Successful.";
            return View("Register", new User());
        }
    }
}