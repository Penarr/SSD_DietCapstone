using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace SmartDietCapstone.Pages
{
    public class IndexModel : PageModel
    {
        [Required]
        [BindProperty]
        [Range(1, 115)]
        public int age { get; set; }


        [Required]
        [BindProperty]
        [Range(1,1000)]
        public double weight { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _client;

        
        public IndexModel(ILogger<IndexModel> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<IActionResult> OnPostAsync(string genderSelect, int age, double weight, int feetSelect, int inchSelect, int activitySelect, int goalSelect, bool isGlutenFree, bool isPescatarian, bool isVegetarian, bool isVegan, bool isKeto, int carbNumSelect, int mealNumSelect)
        {
            double centimetres = feetSelect * 30.48 + (inchSelect * 2.54);
            double kilograms = weight / 2.20462;


            double height = inchSelect + feetSelect * 12;



            FoodCalculator foodCalculator = new FoodCalculator(_client, genderSelect, age, weight, height, goalSelect, activitySelect, isKeto, carbNumSelect);

            var diet = await foodCalculator.GenerateDiet(3);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                errors = errors.ToList();
               
                return new PageResult();
            }

           
            return new RedirectToPageResult("Diet");

            

        }


       
    }
}
