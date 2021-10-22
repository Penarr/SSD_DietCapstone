using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartDietCapstone.Areas.Identity.Data;
using SmartDietCapstone.Models;

namespace SmartDietCapstone.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<SmartDietCapstoneUser> _userManager;
        private readonly SignInManager<SmartDietCapstoneUser> _signInManager;
        private readonly IConfiguration _configuration;
        public List<List<Meal>> favouriteDiets;
        public IndexModel(
            UserManager<SmartDietCapstoneUser> userManager,
            SignInManager<SmartDietCapstoneUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            favouriteDiets = new List<List<Meal>>();
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(SmartDietCapstoneUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
           
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            if (HttpContext.Session.Keys.Contains("favouriteDiet"))
                await SaveFavouriteDiet();

            GetFavouriteDiets();
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (HttpContext.Session.Keys.Contains("favouriteDiet"))
               await SaveFavouriteDiet();
            GetFavouriteDiets();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task SaveFavouriteDiet()
        {
            if (HttpContext.Session.Keys.Contains("favouriteDiet"))
            {
                string jsonDiet = HttpContext.Session.GetString("favouriteDiet");

                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                var user = await _userManager.GetUserAsync(User);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string updateQuery = "INSERT INTO Diet (UserId, SerializedDiet) VALUES(@id, @diet);";
                    SqlCommand command = new SqlCommand(updateQuery, conn);

                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@diet", jsonDiet);

                    try
                    {
                        await conn.OpenAsync();
                        var result = await command.ExecuteNonQueryAsync();
                        conn.Close();
                    }

                    
                    catch (Exception e) 
                    { var message = e.Message; }
                }
                HttpContext.Session.Remove("favouriteDiet");
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dietIndex"></param>
        /// <returns></returns>
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
                var user = await _userManager.GetUserAsync(User);
                command.Parameters.AddWithValue("@id", user.Id);

                try
                {
                    await conn.OpenAsync();
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        favouriteDiets.Add(JsonConvert.DeserializeObject<List<Meal>>(reader.GetString(0)));
                    }

                    if(favouriteDiets != null)
                    {
                        foreach(List<Meal> diet in favouriteDiets)
                        {

                        }
                    }
                }
                catch (Exception e) { }


            }
        }
    }
}
