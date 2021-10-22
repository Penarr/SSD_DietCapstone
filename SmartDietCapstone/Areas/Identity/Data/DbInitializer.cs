using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartDietCapstone.Areas.Identity.Data;
using SmartDietCapstone.Data;
using SSD_Lab1.Data;
using SSD_Lab1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSD_Lab1.Data
{
    public static class DbInitializer
    {
       // public static AppSecrets appSecrets { get; set; }
        public static async Task<int> SeedUsersAndRoles(IServiceProvider serviceProvider)
        {
            // create the database if it doesn't exist
            var context = serviceProvider.GetRequiredService<SmartDietCapstoneContext>();
            context.Database.Migrate();

            var userManager = serviceProvider.GetRequiredService<UserManager<SmartDietCapstoneUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            // Check if roles already exist and exit if there are
            if (roleManager.Roles.Count() > 0)
                return 1;  // should log an error message here

            // Seed roles
            int result = await SeedRoles(roleManager);
            if (result != 0)
                return 2;  // should log an error message here

            // Check if users already exist and exit if there are
            if (userManager.Users.Count() > 0)
                return 3;  // should log an error message here

            // Seed users
            result = await SeedUsers(userManager);
            if (result != 0)
                return 4;  // should log an error message here

            return 0;
        }

        
        private static async Task<int> SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Create Manager Role
            var result = await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!result.Succeeded)
                return 1;  // should log an error message here

            // Create Player Role
            result = await roleManager.CreateAsync(new IdentityRole("User"));
            if (!result.Succeeded)
                return 2;  // should log an error message here

            return 0;
        }

        private static async Task<int> SeedUsers(UserManager<SmartDietCapstoneUser> userManager)
        {
            // Create Manager User
            var admin = new SmartDietCapstoneUser
            {
                UserName = "penarr@dietcapstone.ca",
                Email = "penarr@dietcapstone.ca",
                //FirstName = "Rob",
                //LastName = "Peña",
                //PhoneNumber = "123456789",
                //BirthDate = DateTime.Today,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "T@t5u_M4k1");
            if (!result.Succeeded)
                return 1;  // should log an error message here

            // Assign user to Admin role
            result = await userManager.AddToRoleAsync(admin, "Admin");
            if (!result.Succeeded)
                return 2;  // should log an error message here

            // Create Player User
            //var playerUser = new SmartDietCapstoneUser
            //{
            //    UserName = "the.member@mohawkcollege.ca",
            //    Email = "the.member@mohawkcollege.ca",
            //    FirstName = "The",
            //    LastName = "Player",
            //    PhoneNumber = "123456789",
            //    EmailConfirmed = true
            //};
            //result = await userManager.CreateAsync(playerUser, appSecrets.UserPwd);
            //if (!result.Succeeded)
            //    return 3;  // should log an error message here

            //// Assign user to Member role
            //result = await userManager.AddToRoleAsync(playerUser, "Player");
            //if (!result.Succeeded)
            //    return 4;  // should log an error message here

            return 0;
        }
    }
}
