using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartDietCapstone.Helpers;
using SmartDietCapstone.Models;

namespace SmartDietCapstone.Pages
{
    public class EditMealModel : PageModel
    {
        private APICaller caller;
        public Meal meal;
        public List<Food> searchedFoods;
        public EditMealModel(HttpClient _client, IConfiguration _configuration)
        {
            caller = new APICaller(_configuration["Secrets:FDCApi"], _configuration["Secrets:FDCApiKey"],  _client);
            
        }
        public void OnGet()
        {
            if (TempData.ContainsKey("meal"))
            {
                var jsonMeal = TempData["meal"] as string;
                meal = JsonConvert.DeserializeObject<Meal>(jsonMeal);
            }

        }


        public async Task<JsonResult> OnGetFoodSearch(string query)
        {
            searchedFoods = await caller.GetListOfSearchedFoods(query);
            return  new JsonResult(searchedFoods);
        }

        public async void AddFoodToMeal(Food food)
        {

        }
    }
}
