using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TriviaApp.TriviaAPI;
using TriviaApp.Models;

namespace TriviaApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var Trivia = new Client();
            var TriviaResultList = new List<TriviaResult>();
            var Search = new Search
            {
                Amount = 10,
                Difficulty = "medium"
            };
            var results = Trivia.GetTrivia(Search);

            foreach(var thing in results["results"])
            {
                var IncorrectList = new List<string>();
                foreach (var wrong in thing["incorrect_answers"])
                {
                    IncorrectList.Add((string)wrong);
                }
                TriviaResultList.Add(new TriviaResult()
                {
                    Category = (string)thing["category"],
                    Type = (string)thing["type"],
                    Difficulty = (string)thing["difficulty"],
                    Question = (string)thing["question"],
                    CorrectAnswer = (string)thing["correct_answer"],
                    IncorrectAnswers = IncorrectList,
                });
            }
            return View(TriviaResultList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}