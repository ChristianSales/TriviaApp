using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TriviaApp.Models;
using System.Security.Cryptography;

namespace TriviaApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            if (Session["userID"] != null)
            {
                return RedirectToAction("index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Authorize(User UserModel)
        {

            using (TriviaDBEntities db = new TriviaDBEntities())
            {
                
 
                var userInfo = db.Users.Where(x => x.UserName == UserModel.UserName).FirstOrDefault();
                if (userInfo == null)
                {
                    UserModel.LoginErrorMessage = "Wrong username or password.";
                    return View("index", UserModel);
                }
                else
                {
                    string savedPasswordHash = userInfo.Password;
                    byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                    byte[] salt = new byte[16];
                    Array.Copy(hashBytes, 0, salt, 0, 16);
                    var deriveBytes = new Rfc2898DeriveBytes(UserModel.Password, salt, 10000);
                    byte[] hash = deriveBytes.GetBytes(20);

                    for (int i = 0; i < 20; i++)
                    {
                        if (hashBytes[i + 16] != hash[i])
                        {
                            UserModel.LoginErrorMessage = "Wrong username or password.";
                            return View("index", UserModel);
                        }
                    }
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
            if (Session["userID"] != null)
            {
                return RedirectToAction("index", "Home");
            }

            User UserModel = new User();
            return View(UserModel);
        }

        [HttpPost]
        public ActionResult Register(User UserModel)
        {
            if (Session["userID"] != null)
            {
                return RedirectToAction("index", "Home");
            }

            using (TriviaDBEntities dbModel = new TriviaDBEntities())
            {
                if(dbModel.Users.Any(x => x.UserName == UserModel.UserName))
                {
                    ViewBag.DuplicateName = "Username is taken";
                    return View("Register", UserModel);
                }

                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                var hashed = new Rfc2898DeriveBytes(UserModel.Password, salt, 10000);
                byte[] HashValue = hashed.GetBytes(20);
                byte[] hashbytes = new byte[36];
                Array.Copy(salt, 0, hashbytes, 0, 16);
                Array.Copy(HashValue, 0, hashbytes, 16, 20);
                string PasswordHash = Convert.ToBase64String(hashbytes);
                UserModel.Password = PasswordHash;
                UserModel.ConfirmPassword = PasswordHash;

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