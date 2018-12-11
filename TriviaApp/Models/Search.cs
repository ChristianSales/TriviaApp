using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TriviaApp.Models
{
    public class Search
    {
        public int Amount { get; set; }
        public int Category { get; set; }
        public string Difficulty { get; set; }
    }
}