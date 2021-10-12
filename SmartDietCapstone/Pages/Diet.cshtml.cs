using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft;
using Newtonsoft.Json;

namespace SmartDietCapstone.Pages
{
    public class DietModel : PageModel
    {
        private HttpClient _client;
        public List<List<Food>> diet;
        public DietModel(HttpClient client)
        {
            _client = client;
            
           
            

        }
        public void OnGet()
        {
            var _diet = "";
            if (TempData.ContainsKey("diet"))
            {
                _diet = TempData["diet"] as string;
            }

            this.diet = JsonConvert.DeserializeObject<List<List<Food>>>(_diet);
           
        }

        public void GenerateDiet()
        {
            
        }

       

        

    }
}
