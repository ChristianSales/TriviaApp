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
                var ChoiceList = new List<string>();
                foreach (var wrong in thing["incorrect_answers"])
                {
                    ChoiceList.Add((string)wrong);
                }
                ChoiceList.Add((string)thing["correct_answer"]);
                ChoiceList.Shuffle();
                TriviaResultList.Add(new TriviaResult()
                {
                    Category = (string)thing["category"],
                    Type = (string)thing["type"],
                    Difficulty = (string)thing["difficulty"],
                    Question = (string)thing["question"],
                    CorrectAnswer = (string)thing["correct_answer"],
                    Choices = ChoiceList,
                });
            }
            ViewBag.Count = 0;
            Session["Trivia"] = TriviaResultList;
            Session["Index"] = 0;
            return View(TriviaResultList[(int)Session["Index"]]);
        }

        public ActionResult Quiz(TriviaResult result)
        {
            string correct = result.CorrectAnswer;
            string selected = result.SelectedAnswer;
            List<TriviaResult> Quiz = (List<TriviaResult>)Session["Trivia"];
            Quiz[(int)Session["Index"]].SelectedAnswer = result.SelectedAnswer;


            if (correct == selected)
            {
                Quiz[(int)Session["Index"]].Score = true;
            }
            
            Session["Index"] = (int)Session["Index"] + 1;
            Session["Trivia"] = Quiz;
            if(Quiz.Count() == (int)Session["Index"])
            {
                return View("Results");
            }
            return View("Index", Quiz[(int)Session["Index"]]);
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