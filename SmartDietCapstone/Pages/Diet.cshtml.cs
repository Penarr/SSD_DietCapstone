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

using SmartDietCapstone.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace SmartDietCapstone.Pages
{

    public class DietModel : PageModel
    {

        public List<Meal> _diet;
        public FoodCalculator foodCalculator;
        public double dietCalories;
        public double recommendedCalories;
        private readonly UserManager<SmartDietCapstoneUser> _userManager;
        
        public DietModel(UserManager<SmartDietCapstoneUser> userManager)
        {

            _userManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>

        public async Task OnGet()
        {
            await SetDietAndCalculator();
            dietCalories = 0;
            if (_diet != null)
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
        private async Task SetDietAndCalculator()
        {
            var diet = "";
            var calculator = "";

            if (HttpContext.Session.Keys.Contains("diet"))
            {
                diet = HttpContext.Session.GetString("diet");
                _diet = JsonConvert.DeserializeObject<List<Meal>>(diet);
            }



            if (HttpContext.Session.Keys.Contains("calculator")) // Saves most recent recommendations to user table
            {
                calculator = HttpContext.Session.GetString("calculator");
                foodCalculator = JsonConvert.DeserializeObject<FoodCalculator>(calculator);
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    user.UserCalories = foodCalculator.calorieCount;
                    user.UserProtein = foodCalculator.proteinCount;
                    user.UserCarbs = foodCalculator.carbCount;
                    user.UserFat = foodCalculator.fatCount;
                    await _userManager.UpdateAsync(user);
                }
                recommendedCalories = foodCalculator.calorieCount;
            }
           if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                recommendedCalories = user.UserCalories;
            }
               

        }


       
        /// <summary>
        /// Saves diet to database as favourite diet if user is logged in
        /// </summary>
        /// <returns>Favourite diets page</returns>
        public async Task<IActionResult> OnPostSaveDiet()
        {
            await SetDietAndCalculator();
            if (_diet.Count > 0)
            {
                HttpContext.Session.SetString("favouriteDiet", HttpContext.Session.GetString("diet"));

                return new RedirectToPageResult("/Account/Manage/FavouriteDiets", "SaveDiet", new { area = "Identity" });
            }

            return new PageResult();

        }
        /// <summary>
        /// Opens the edit meal page to edit meal
        /// </summary>
        /// <param name="mealIndex">Index of the meal in the diet list</param>
        /// <returns>Edit meal page result</returns>

        public async Task<IActionResult> OnPostGoToEditMeal(int mealIndex)
        {
            await SetDietAndCalculator();
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
        public async Task<IActionResult> OnPostDeleteMeal(int deleteIndex)
        {
            await SetDietAndCalculator();
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
        public async Task OnGetEditDiet()
        {
            await SetDietAndCalculator();

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
