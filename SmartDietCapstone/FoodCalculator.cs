using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using System.Threading.Tasks;

namespace SmartDietCapstone
{/// <summary>
/// FOUNDATION FOODS DOCUMENTATION https://fdc.nal.usda.gov/docs/Foundation_Foods_Documentation_Apr2021.pdf
/// API DOCUMENTATION https://fdc.nal.usda.gov/api-spec/fdc_api.html
/// </summary>
    public class FoodCalculator
    {

        private int proteinApiNum = 203;
        private int carbApiNum = 305;
        private int fatApiNum = 204;
        private int calApiNum = 208;
        private string dataType = "Foundation,SR%20Legacy";

        private double calorieCount;
        private double fatCount;
        private double proteinCount;
        private double carbCount;


        private HttpClient _client;
        public FoodCalculator(HttpClient client, string gender, int age, double weight, double height, int goal, int activityLevel, bool isKeto, int carbAmount)
        {
            _client = client;
            calorieCount = CalculateCalories( gender, age,  weight, height, goal, activityLevel);
            fatCount = CalculateFat(calorieCount, carbAmount, isKeto);
            proteinCount = CalculateProtein(calorieCount);
            carbCount = CalculateCarbs(calorieCount, carbAmount, isKeto);

            GenerateDiet(3);
            //object o = SearchFood("meat");
        }
        /// <summary>
        /// Implements Benedict-Harris method of calculating calories based on physiology and activity levels.
        /// Assumes measurements are imperial, not metric
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="age"></param>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        /// <param name="goal"></param>
        /// <param name="activityLevel"></param>
        /// <returns></returns>
        public double CalculateCalories(string gender, int age, double weight, double height, int goal, int activityLevel)
        {
            double calories = 2000;
            if (gender == "Male")
                calories = 66.47 + (6.24 * weight) + (12.7 * height) - (6.755 * age);


            else if (gender == "Female")
                calories = 655.1 + (4.35 * weight) + (4.7 * height) - (4.7 * age);

            switch (activityLevel) {
                case 0:
                    calories *= 1.2;
                    break;

                case 1:
                    calories *= 1.375;
                    break;
                case 2:
                    calories *= 1.55;
                    break;
                case 3:
                    calories *= 1.725;
                    break;
                case 4:
                    calories *= 1.9;
                    break;
                case 5:
                    calories *= 2;
                    break;
            }

            switch (goal) {
                case 0:
                    calories -= 500;
                    break;

                case 2:
                    calories += 500;
                    break;
            }

            return calories;

           
        }

       

       private async Task<object> SearchFood(string query)
        {
            // string data = "query=apple&datatype=Foundation&pageSize=2&api_key=LFvEHThAZuPapYjKemtarLfGUylkrh1SnDwCdmCA";
            var response = await _client.GetAsync($"https://api.nal.usda.gov/fdc/v1/foods/search?query={query}&dataType=Foundation,SR%20Legacy&pageSize=25&api_key=LFvEHThAZuPapYjKemtarLfGUylkrh1SnDwCdmCA"); // search
            // var response = await _client.GetAsync("https://api.nal.usda.gov/fdc/v1/foods/list?datatype=Foundation&pageSize=25&api_key=LFvEHThAZuPapYjKemtarLfGUylkrh1SnDwCdmCA"); // List
            // response = await _client.PostAsync("https://api.nal.usda.gov/fdc/v1/foods/search", new StringContent(data));


            //get id of protein, carb, fat, kcal and derivation description
            Random rand = new Random();
            var result = await response.Content.ReadAsStringAsync();

            JObject obj;
            JArray jarray;
            var x = new object();
            JsonSerializer jsonSerializer = new JsonSerializer();
            try
            {
                obj = jsonSerializer.(result);
                int randIndex = rand.Next(0, obj["foods"].Count());
                x = obj["foods"][randIndex]; // Search
            }
            catch
            {
                jarray = JArray.Parse(result); // List
            }
            return x;
        }

        
       public double CalculateFat(double calories, int carbAmount, bool isKeto)
        {
            int fatPercent = 35;

            if (isKeto)
                fatPercent = 60;
            else
            {
                switch (carbAmount)
                {
                    case 1:
                        fatPercent = 45;
                        break;
                    case 2:
                        fatPercent = 35;
                        break;
                    case 3:
                        fatPercent = 25;
                        break;

                }
            }
            
            // Amount of fat in grams
            return calories * fatPercent / 9;

        }


        public double CalculateProtein(double calories)
        {
            // Amount of protein in grams
            return calories * 0.3 / 4;

        }


        public double CalculateCarbs(double calories, int carbAmount, bool isKeto)
        {
            int carbPercent = 55;

            if (isKeto)
                carbPercent = 60;
            else
            {
                switch (carbAmount)
                {
                    case 1:
                        carbPercent = 45;
                        break;
                    case 2:
                        carbPercent = 55;
                        break;
                    case 3:
                        carbPercent = 65;
                        break;

                }
            }

            // Amount of carbs in grams
            return calories * carbPercent / 4;
        }


        public object[,] GenerateDiet(int mealNum)
        {
            double caloriesPerMeal = calorieCount / mealNum;
            string[] queries = { "Meat", "vegetable", "grain" };
            object[,] mealPlan = new string[mealNum, 3];
            for(int i = 0; i <= mealNum; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    mealPlan[i,j] = SearchFood(queries[j]);
                }
                
            }

            return mealPlan;
        }
        


       
    }
}
