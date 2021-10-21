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
using Microsoft.AspNetCore.Authorization;

namespace SmartDietCapstone.Pages
{

    public class DietModel : PageModel
    {

        public List<Meal> _diet;
        public FoodCalculator foodCalculator;
        public double dietCalories;
        public DietModel() { }

        /// <summary>
        /// 
        /// </summary>

        public void OnGet()
        {
            SetDietAndCalculator();
            dietCalories = 0;
            if(_diet != null)
            {
                foreach (Meal meal in _diet)
                {
                    foreach (Food food in meal.foods)
                    {
                        dietCalories += food.cals;
                    }
                }
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetDietAndCalculator()
        {
            var diet = "";
            var calculator = "";

            if (HttpContext.Session.Keys.Contains("diet"))
                diet = HttpContext.Session.GetString("diet");


            if (HttpContext.Session.Keys.Contains("calculator"))
                calculator = HttpContext.Session.GetString("calculator");

            _diet = JsonConvert.DeserializeObject<List<Meal>>(diet);
            foodCalculator = JsonConvert.DeserializeObject<FoodCalculator>(calculator);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="mealIndex"></param>
        /// <returns></returns>\

        public IActionResult OnPostGoToEditMeal(int mealIndex)
        {
            SetDietAndCalculator();
            if (_diet.Count > mealIndex)
            {
                Meal meal = _diet[mealIndex];

                string jsonMeal = JsonConvert.SerializeObject(meal);
                HttpContext.Session.SetInt32("mealIndex", mealIndex);
                TempData["meal"] = jsonMeal;
                return new RedirectToPageResult("EditMeal");

            }

            return new PageResult();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dietIndex"></param>
        /// <returns></returns>
        public IActionResult OnPostDeleteMeal(int deleteIndex)
        {
            SetDietAndCalculator();
            if (_diet.Count > deleteIndex)
            {
                _diet.RemoveAt(deleteIndex);

                HttpContext.Session.SetString("diet", JsonConvert.SerializeObject(_diet));
                return new RedirectToPageResult("Diet");

            }


            return new PageResult();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnGetEditDiet()
        {
            SetDietAndCalculator();

            if (HttpContext.Session.Keys.Contains("mealIndex"))
            {
                int mealIndex = (int)HttpContext.Session.GetInt32("mealIndex");
                if (TempData.ContainsKey("meal"))
                {
                    _diet[mealIndex] = JsonConvert.DeserializeObject<Meal>(TempData["meal"] as string);
                    HttpContext.Session.SetString("diet", JsonConvert.SerializeObject(_diet));
                }


            }

        }














    }
}
