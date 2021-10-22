using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartDietCapstone.Helpers;
using SmartDietCapstone.Models;

namespace SmartDietCapstone.Pages
{
    [Authorize]
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


        
        public  ActionResult OnPostValidateMeal(string jsonFoods)
        {
            List<Food> foods = JsonConvert.DeserializeObject<List<Food>>(jsonFoods);
            if (foods.Count > 0)
            {
                Meal meal = new Meal();
                meal.foods = foods;
                foreach(Food food in meal.foods)
                {
                    meal.totalCals += food.cals;
                    meal.totalProtein += food.protein;
                    meal.totalCarbs += food.carbs;
                    meal.totalFat += food.fat;
                }

                TempData["meal"] = JsonConvert.SerializeObject(meal);

                return new RedirectToPageResult("Diet", "EditDiet");
            }
            return new PageResult();

        }
    }
}
