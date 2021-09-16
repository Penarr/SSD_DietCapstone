using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft;
namespace SmartDietCapstone.Pages
{
    public class DietModel : PageModel
    {
        HttpClient _client;
        public  DietModel(HttpClient client)
        {
            _client = client;
        }
        public void OnGet()
        {
        }

        public void GenerateDiet()
        {
            
        }

       

        

    }
}
