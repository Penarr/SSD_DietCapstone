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
        // Nutrient number values in FDC api
        private const int proteinApiNum = 203;
        private const int carbApiNum = 205;
        private const int fatApiNum = 204;
        private const int calApiNum = 208;
        private string apiKey;
        private string apiUrl;

        
        private const string dataType = "Foundation,SR%20Legacy";
        // Ideal amount in grams of each meal
        private double calorieCount;
        private double fatCount;
        private double proteinCount;
        private double carbCount;


        private HttpClient _client;
        public FoodCalculator(HttpClient client, string gender, int age, double weight, double height, int goal, int activityLevel, bool isKeto, int carbAmount, string apiUrl, string apiKey)
        {
            _client = client;
            
            calorieCount = CalculateCalories(gender, age, weight, height, goal, activityLevel);
            fatCount = CalculateFat(calorieCount, carbAmount, isKeto);
            proteinCount = CalculateProtein(calorieCount);
            carbCount = CalculateCarbs(calorieCount, carbAmount, isKeto);
            this.apiKey = apiKey;
            this.apiUrl = apiUrl;
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
            if (gender == "male")
                calories = 66.47 + (6.24 * weight) + (12.7 * height) - (6.755 * age);


            else if (gender == "female")
                calories = 655.1 + (4.35 * weight) + (4.7 * height) - (4.7 * age);

            switch (activityLevel)
            {
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

            switch (goal)
            {
                case 0:
                    calories -= 500;
                    break;

                case 2:
                    calories += 500;
                    break;
            }

            return 2000;


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="j"></param>
        /// <param name="caloriesRemaining"></param>
        /// <param name="proteinRemaing"></param>
        /// <param name="fatRemaining"></param>
        /// <param name="carbsRemaining"></param>
        /// <param name="mealNum"></param>
        /// <returns></returns>
        private async Task<Food> SearchFood(string query, int j, double caloriesRemaining, double proteinRemaing, double fatRemaining, double carbsRemaining, int mealNum)
        {
          
            var response = await _client.GetAsync($"{apiUrl}search?query={query}&dataType=Foundation&pageSize=25&api_key={apiKey}"); // search
          


            //get id of protein, carb, fat, kcal and derivation description
            Random rand = new Random();
            var result = await response.Content.ReadAsStringAsync();

            // JObject that will store information in an array from api
            JObject obj;

            Food food = new Food();

            try
            {
                bool validFoodChoice = false;
                // Try food again
                while (!validFoodChoice)
                {
                    obj = JObject.Parse(result);
                    int randIndex = rand.Next(0, obj["foods"].Count());

                    food.name = obj["foods"][randIndex]["description"].ToString();
                    var foodNutrients = obj["foods"][randIndex]["foodNutrients"];
                    double calsPerGram = 0;
                    double proteinPerGram = 0;
                    double fatPerGram = 0;
                    double carbsPerGram = 0;
                    // Iterate through  nutrient value, assign value per gram to variable
                    for (int i = 0; i < foodNutrients.Count(); i++)
                    {
                        int nutrientNumber = (int)foodNutrients[i]["nutrientNumber"];
                        switch (nutrientNumber)
                        {
                            case calApiNum:
                                calsPerGram = (double)foodNutrients[i]["value"] / 100;
                                break;

                            case proteinApiNum:
                                proteinPerGram = (double)foodNutrients[i]["value"] / 100;
                                break;
                            case carbApiNum:
                                carbsPerGram = (double)foodNutrients[i]["value"] / 100;
                                break;
                            case fatApiNum:
                                fatPerGram = (double)foodNutrients[i]["value"] / 100;
                                break;
                        }
                        if (fatPerGram != 0 && proteinPerGram != 0 && carbsPerGram != 0 && calsPerGram != 0)
                            break;
                    }

                    // This section will decide serving size of food
                    double calsPerMeal = calorieCount / mealNum;
                    double proteinPerMeal = proteinCount / mealNum;
                    double fatPerMeal = fatCount / mealNum;
                    double carbsPerMeal = carbCount / mealNum;
                    // Calculate serving size of food in meal


                    // Protein
                    if (j == 0)
                    {
                        double servingSize = (proteinPerMeal - proteinPerMeal * 0.05) / proteinPerGram;


                        double caloriesOfFood = calsPerGram * servingSize;
                        
                            
                        food.servingSize = servingSize;
                        food.carbs = carbsPerGram * servingSize;
                        food.protein = proteinPerGram * servingSize;
                        food.fat = fatPerGram * servingSize;
                        food.cals = calsPerGram * servingSize;
                        if (!(food.carbs >= food.protein)! && !(food.fat >= food.protein) || food.cals <= calsPerMeal + calsPerMeal * 0.1)
                            validFoodChoice = true;
                    }
                    // Carb food
                    else if (j == 1)
                    {

                        double servingSize = (carbsRemaining - carbsPerMeal * 0.1) / carbsPerGram;
                        double caloriesOfFood = calsPerGram * servingSize;
                        if (caloriesOfFood > caloriesRemaining + caloriesRemaining * 0.05)
                            servingSize = caloriesRemaining * 0.7 / calsPerGram;
                        food.servingSize = servingSize;
                        food.carbs = carbsPerGram * servingSize;
                        food.protein = proteinPerGram * servingSize;
                        food.fat = fatPerGram * servingSize;
                        food.cals = calsPerGram * servingSize;
                        if (!(food.protein >= food.carbs))
                            validFoodChoice = true;
                    }
                    // Fill with vegetable
                    else if (j == 2)
                    {
                        if (caloriesRemaining > calsPerMeal * 0.1)
                        {
                            double servingSize = caloriesRemaining / calsPerGram;
                            food.servingSize = servingSize;
                            food.carbs = carbsPerGram * servingSize;
                            food.protein = proteinPerGram * servingSize;
                            food.fat = fatPerGram * servingSize;
                            food.cals = calsPerGram * servingSize;
                            validFoodChoice = true;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                var error = e.Message;

            }
            return food;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calories"></param>
        /// <param name="carbAmount"></param>
        /// <param name="isKeto"></param>
        /// <returns></returns>
        public double CalculateFat(double calories, int carbAmount, bool isKeto)
        {
            double fatPercent = 0.35;

            if (isKeto)
                fatPercent = 0.60;
            else
            {
                switch (carbAmount)
                {
                    case 1:
                        fatPercent = 0.45;
                        break;
                    case 2:
                        fatPercent = 0.35;
                        break;
                    case 3:
                        fatPercent = 0.25;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calories"></param>
        /// <param name="carbAmount"></param>
        /// <param name="isKeto"></param>
        /// <returns></returns>
        public double CalculateCarbs(double calories, int carbAmount, bool isKeto)
        {
            double carbPercent = 0.55;

            if (isKeto)
                carbPercent = 0.60;
            else
            {
                switch (carbAmount)
                {
                    case 1:
                        carbPercent = 0.45;
                        break;
                    case 2:
                        carbPercent = 0.55;
                        break;
                    case 3:
                        carbPercent = 0.65;
                        break;

                }
            }

            // Amount of carbs in grams
            return calories * carbPercent / 4;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mealNum"></param>
        /// <returns></returns>
        public async Task<Food[,]> GenerateDiet(int mealNum)
        {

            string[] queries = { "Meat chicken turkey fish", "bread rice potato", "vegetable fruit" };
            Food[,] mealPlan = new Food[mealNum, 3];
            for (int i = 0; i < mealNum; i++)
            {
                // Calories for each meal
                double calsRemaining = calorieCount / mealNum;
                double proteinRemaining = proteinCount / mealNum;
                double carbsRemaining = carbCount / mealNum;
                double fatRemaining = fatCount / mealNum;

                //Individual food generation per meal
                for (int j = 0; j < 3; j++)
                {
                    // Only grab a new food if remaining calories is greater than 5% of the total calorie count
                    if(calsRemaining > calorieCount / mealNum * 0.05)
                    {
                        mealPlan[i, j] = await SearchFood(queries[j], j, calsRemaining, proteinRemaining, fatRemaining, carbsRemaining, mealNum);
                        // Calculate remaining macros for meal
                        calsRemaining -= mealPlan[i, j].cals;
                        proteinRemaining -= mealPlan[i, j].protein;
                        carbsRemaining -= mealPlan[i, j].carbs;
                        fatRemaining -= mealPlan[i, j].fat;
                    }
                    
                }

            }

            double totalCalories = 0;
            double totalProtein = 0;
            double totalCarbs = 0;
            double totalFat = 0;

            for(int i = 0; i < mealPlan.GetLength(0); i++)
            {
                for (int j = 0; j < mealPlan.GetLength(1); j++)
                    if (mealPlan[i, j] != null)
                    {
                        totalCalories += mealPlan[i, j].cals;
                        totalProtein += mealPlan[i, j].protein;
                        totalCarbs += mealPlan[i, j].carbs;
                        totalFat += mealPlan[i, j].fat;
                    }
                        
            }
            return mealPlan;
        }




    }
}
