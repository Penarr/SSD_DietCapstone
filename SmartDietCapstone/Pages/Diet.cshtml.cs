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
        
        public List<Meal> _diet;
        public FoodCalculator foodCalculator;
        public double dietCalories;
        public DietModel() { }


        public void OnGet()
        {
            SetDietAndCalculator();
            dietCalories = 0;
            
            foreach (Meal meal in _diet)
            {
                foreach (Food food in meal.foods)
                {
                    dietCalories += food.cals;
                }
            }
        }
        private void SetDietAndCalculator()
        {
            var diet = "";
            var calculator = "";

            if (HttpContext.Session.Keys.Contains("diet"))
                diet = HttpContext.Session.GetString("diet");


            if (HttpContext.Session.Keys.Contains("calculator"))
                calculator = HttpContext.Session.GetString("calculator");

            this._diet = JsonConvert.DeserializeObject<List<Meal>>(diet);
            this.foodCalculator = JsonConvert.DeserializeObject<FoodCalculator>(calculator);
        }
        public async Task<IActionResult> OnPostTryEditMeal(int mealIndex)
        {
            SetDietAndCalculator();

            Meal meal = _diet[mealIndex];
            string jsonMeal = JsonConvert.SerializeObject(meal);
            TempData["meal"] = jsonMeal;
            return new RedirectToPageResult("EditMeal");
        }

        
       

        

    }
}
