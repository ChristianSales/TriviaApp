using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using TriviaApp.Models;
using System.Net.Http;
using Newtonsoft.Json;


namespace TriviaApp.TriviaAPI
{
    public class Client
    {


        public JToken GetTrivia(Search Options)
        {
 
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://opentdb.com/api.php?amount=");
                HttpResponseMessage Response = client.GetAsync($"https://opentdb.com/api.php?amount={Options.Amount}&category={Options.Category}&difficulty={Options.Difficulty}&type=multiple&encode=url3986").Result;
                string result = Response.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(result);
                return json;
            }
        }
        public JToken GetCategories()
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://opentdb.com/api.php?amount=");
                HttpResponseMessage Response = client.GetAsync($"https://opentdb.com/api_category.php").Result;
                string result = Response.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(result);
                return json;
            }
        }
    }
}