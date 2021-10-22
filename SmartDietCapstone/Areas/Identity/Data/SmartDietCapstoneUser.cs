using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SmartDietCapstone.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the SmartDietCapstoneUser class
    public class SmartDietCapstoneUser : IdentityUser
    {
        public double UserCalories { get; set; }

        public double UserProtein { get; set; }
        public double UserFat { get; set; }

        public double UserCarbs { get; set; }
    }
}
