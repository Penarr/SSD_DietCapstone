using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartDietCapstone.Models;

namespace SmartDietCapstone.Pages
{
    public class ViewDietInformationModel : PageModel
    {


        private IConfiguration _configuration;
        List<List<Meal>> favouriteDiets;
        public ViewDietInformationModel(IConfiguration configuration)
        {
            _configuration = configuration;
            favouriteDiets = new List<List<Meal>>();
            GetFavouriteDiets();
        }

        public void OnGet()
        {
            GetFavouriteDiets();
        }

        /// <summary>
        /// 
        /// </summary>
        public async void OnPostSaveDiet()
        {
            if (HttpContext.Session.Keys.Contains("favouriteDiet"))
            {
                string jsonDiet = HttpContext.Session.GetString("favouriteDiets");

                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string updateQuery = "INSERT INTO Diet (UserId, SerializedDiet) VALUES(@id, @diet);";
                    SqlCommand command = new SqlCommand(updateQuery, conn);

                    command.Parameters.AddWithValue("@id", User.Identity.GetUserId());
                    command.Parameters.AddWithValue("@diet", jsonDiet);

                    try
                    {
                        await conn.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception e) { }
                }
            }

            GetFavouriteDiets();
        }


        public IActionResult EditFavouriteDiet(int dietIndex)
        {
            if (dietIndex < favouriteDiets.Count)
            {
                string jsonDiet = JsonConvert.SerializeObject(favouriteDiets[dietIndex]);
                HttpContext.Session.SetString("diet", jsonDiet);

            }




            return new PageResult();
        }
        /// <summary>
        /// 
        /// </summary>
        public async void GetFavouriteDiets()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SerializedDiet from Diet where UserId = @id;";
                SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("@id", User.Identity.GetUserId());

                try
                {
                    await conn.OpenAsync();
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        favouriteDiets.Add(JsonConvert.DeserializeObject<List<Meal>>(reader.GetString(0)));
                    }
                }
                catch (Exception e) { }


            }
        }
    }
}
