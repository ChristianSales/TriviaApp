using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TriviaApp.TriviaAPI;
using TriviaApp.Models;
using System.Net.Http;

namespace TriviaApp.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("index", "Account");
            }

            Search model = new Search();
            return View(model);
        }
        public ActionResult QuizStart(Search SearchOptions)
        {
            if(Session["userID"] == null)
            {
                return RedirectToAction("index", "Account");
            }
            
            var Trivia = new Client();
            var TriviaResultList = new List<TriviaResult>();
            var results = Trivia.GetTrivia(SearchOptions);
           

            foreach(var thing in results["results"])
            {
                var ChoiceList = new List<string>();
                foreach (var wrong in thing["incorrect_answers"])
                {
                    ChoiceList.Add(HttpContext.Server.UrlDecode((string)wrong));
                }
                ChoiceList.Add(HttpContext.Server.UrlDecode((string)thing["correct_answer"]));
                ChoiceList.Shuffle();
                TriviaResultList.Add(new TriviaResult()
                {
                    Category = (string)thing["category"],
                    Type = (string)thing["type"],
                    Difficulty = (string)thing["difficulty"],
                    Question = HttpContext.Server.UrlDecode((string)thing["question"]),
                    CorrectAnswer = HttpContext.Server.UrlDecode((string)thing["correct_answer"]),
                    Choices = ChoiceList,
                });
            }
            if(TriviaResultList.Count() == 0)
            {
                return RedirectToAction("Index","Home");
            }
            Session["Trivia"] = TriviaResultList;
            Session["Index"] = 0;
            Session["Correct"] = 0;
            return View("Quiz", TriviaResultList[(int)Session["Index"]]);
        }

        public ActionResult Quiz(TriviaResult result)
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("index", "Account");
            }

            List<TriviaResult> Quiz = (List<TriviaResult>)Session["Trivia"];
            string correct = Quiz[(int)Session["Index"]].CorrectAnswer;
            string selected = result.SelectedAnswer;
            Quiz[(int)Session["Index"]].SelectedAnswer = selected; 


            if (correct == selected)
            {
                Session["Correct"] = (int)Session["Correct"] + 1;
                Quiz[(int)Session["Index"]].Score = true;
            }

            Session["Index"] = (int)Session["Index"] + 1;
            Session["Trivia"] = Quiz;
            var count = (int)Session["index"];
            if (Quiz.Count() == (int)Session["Index"])
            {
                using (TriviaDBEntities dbmodel = new TriviaDBEntities())
                {
                    var userID = (int)Session["userID"];
                    var user = dbmodel.Users.SingleOrDefault(m => m.UserID == userID);
                    if(user != null)
                    {
                        var wrong = Quiz.Count() - (int)Session["Correct"];
                        var TotalWrong = user.Stat.AmountWrong;
                        TotalWrong = TotalWrong + wrong;
                        user.Stat.AmountWrong = TotalWrong;
                        var TotalCorrect = user.Stat.AmountCorrect;
                        TotalCorrect = TotalCorrect + (int)Session["Correct"];
                        user.Stat.AmountCorrect = TotalCorrect;
                        var TotalQuiz = user.Stat.QuizTaken;
                        TotalQuiz++;
                        user.Stat.QuizTaken = TotalQuiz;
                        dbmodel.SaveChanges();
                    }
                }
                return RedirectToAction("Results", "Home");
            }
            return View("Quiz", Quiz[(int)Session["Index"]]);
        }

        public ActionResult Results()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("index", "Account");
            }

            List<TriviaResult> FinishedQuiz = (List<TriviaResult>)Session["Trivia"];


            return View();
        }

        public ActionResult MyStats()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("index", "Account");
            }

            User UserAccount = new User();
            Stat UserStat = new Stat();
            var userID = (int)Session["userID"];
            using (TriviaDBEntities dbModel = new TriviaDBEntities())
            {
                UserAccount = dbModel.Users.FirstOrDefault(x => x.UserID == userID);
                UserStat = dbModel.Stats.FirstOrDefault(x => x.StatsID == UserAccount.StatsID);

            }
            UserAccount.Stat = UserStat;
            return View(UserAccount);
        }
    }
}