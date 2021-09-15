using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDietCapstone.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
           
        }

        public IActionResult OnPost(string genderSelect, int age, double weight, int feetSelect, int inchSelect, int activitySelect, int goalSelect, bool isGlutenFree, bool isPescatarian, bool isVegetarian, bool isVegan, bool isKeto, int carbNumSelect, int mealNumSelect)
        {

            FoodCalculator foodCalculator = new FoodCalculator();

            // Convert imperial units to metric
            double centimetres = feetSelect * 30.48 + (inchSelect * 2.54);
            double kilograms = weight / 2.20462;

            //Add feet to inches
            double height = inchSelect + feetSelect * 12;

            double calories = foodCalculator.CalculateCalories(genderSelect, age, weight, height, goalSelect, activitySelect);


            if (!ModelState.IsValid)
            {
                return new PageResult();
            }
            return new RedirectToPageResult("Diet");

            

        }


       
    }
}
