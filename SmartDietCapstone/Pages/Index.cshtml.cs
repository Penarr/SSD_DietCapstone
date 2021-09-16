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

            FoodCalculator foodCalculator = new FoodCalculator();

            // Convert imperial units to metric
            double centimetres = feetSelect * 30.48 + (inchSelect * 2.54);
            double kilograms = weight / 2.20462;

            //Add feet to inches
            double height = inchSelect + feetSelect * 12;

            double calories = foodCalculator.CalculateCalories(genderSelect, age, weight, height, goalSelect, activitySelect);


            // Testing api feature
            //string data = "query=apple&datatype=Foundation&pageSize=2&api_key=LFvEHThAZuPapYjKemtarLfGUylkrh1SnDwCdmCA";
            var response = await _client.GetAsync("https://api.nal.usda.gov/fdc/v1/foods/search?query=apple&datatype=foundation&api_key=LFvEHThAZuPapYjKemtarLfGUylkrh1SnDwCdmCA"); // search
            //var response = await _client.GetAsync("https://api.nal.usda.gov/fdc/v1/foods/list?datatype=Foundation&pageSize=25&api_key=LFvEHThAZuPapYjKemtarLfGUylkrh1SnDwCdmCA"); // List
            //response = await _client.PostAsync("https://api.nal.usda.gov/fdc/v1/foods/search", new StringContent(data));


            //get id of protein, carb, fat, kcal and derivation description

            var result = await response.Content.ReadAsStringAsync();

            JObject obj;
            JArray jarray;
            try
            {
                 obj = JObject.Parse(result);
                var x = obj["foods"][0]; // Search
            }
            catch
            {
                jarray = JArray.Parse(result); // List
            }

            

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
