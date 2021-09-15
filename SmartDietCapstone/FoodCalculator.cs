using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDietCapstone
{
    public class FoodCalculator
    {
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
            double calories = 0;
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

       public double CalculateFat(double calories, int carbAmount)
        {

            return 0;

        }


        public double CalculateProtein(double calories, int carbAmount)
        {
            return 0;
        }


        public double CalculateCarbs(double calories, int carbAmount)
        {
            return 0;
        }
    }
}
