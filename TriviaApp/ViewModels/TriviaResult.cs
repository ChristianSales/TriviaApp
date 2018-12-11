using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TriviaApp.Models
{
    public class TriviaResult
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }

    }
}