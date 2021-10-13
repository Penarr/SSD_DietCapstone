using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft;
using Newtonsoft.Json;
using SmartDietCapstone.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;

namespace SmartDietCapstone.Pages
{
    public class DietModel : PageModel
    {
        private HttpClient _client;
        public List<Meal> diet;
        public FoodCalculator foodCalculator;
        public double dietCalories;
        public DietModel(HttpClient client)
        {
            _client = client;
            
            

        }
        public void OnGet()
        {
            var _diet = "";
            var _calculator = "";
            if (HttpContext.Session.Keys.Contains("diet"))
            {
                _diet = HttpContext.Session.GetString("diet");
            }
            if (HttpContext.Session.Keys.Contains("calculator"))
            {
                _calculator = HttpContext.Session.GetString("calculator");
            }
            this.diet = JsonConvert.DeserializeObject<List<Meal>>(_diet);
            this.foodCalculator = JsonConvert.DeserializeObject<FoodCalculator>(_calculator);
            dietCalories = 0;
            foreach (Meal meal in diet)
            {
                foreach (Food food in meal.foods)
                {
                    dietCalories += food.cals;
                }
            }
        }

       

        

    }
}
