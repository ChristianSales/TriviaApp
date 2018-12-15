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
            Search model = new Search();
            return View(model);
        }
        public ActionResult QuizStart(Search SearchOptions)
        {
            
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
            Session["Trivia"] = TriviaResultList;
            Session["Index"] = 0;
            return View("Quiz", TriviaResultList[(int)Session["Index"]]);
        }

        public ActionResult Quiz(TriviaResult result)
        {
            
            
            List<TriviaResult> Quiz = (List<TriviaResult>)Session["Trivia"];
            string correct = Quiz[(int)Session["Index"]].CorrectAnswer;
            string selected = result.SelectedAnswer;
            Quiz[(int)Session["Index"]].SelectedAnswer = selected; 


            if (correct == selected)
            {
                Quiz[(int)Session["Index"]].Score = true;
            }

            Session["Index"] = (int)Session["Index"] + 1;
            Session["Trivia"] = Quiz;
            var count = (int)Session["index"];
            if (Quiz.Count() == (int)Session["Index"])
            {
               return RedirectToAction("Results", "Home");
            }
            return View("Quiz", Quiz[(int)Session["Index"]]);
        }

        public ActionResult Results()
        {
            return View();
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