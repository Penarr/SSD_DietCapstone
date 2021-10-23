using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartDietCapstone.Areas.Identity.Data;

namespace SmartDietCapstone.Areas.Identity.Pages.Account.Manage
{
    public class AdminInfoModel : PageModel
    {

        internal UserManager<SmartDietCapstoneUser> _userManager;
        public AdminInfoModel(UserManager<SmartDietCapstoneUser> userManager)
        {
            _userManager = userManager;
        }
        public List<double> usersCalories;
        public List<double> usersProtein;
        public List<double> usersCarbs;
        public List<double> usersFat;
        public void OnGet()
        {
        }


      

        
    }
}
