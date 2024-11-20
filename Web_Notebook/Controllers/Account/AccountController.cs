using Microsoft.AspNetCore.Mvc;
using System.Text;
using Web_Notebook.Models.Auth;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Web_Notebook.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController()
        {
            _httpClient = new HttpClient();
        }

        // GET: /Account/SignUp
        [HttpGet]
        public IActionResult SignUp()
        {
          
            return View(); // Load SignUp view
        }

        // POST: /Account/SignUp
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
           
            // Serialize the model to JSON
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Call the API to sign up the user
            var response = await _httpClient.PostAsync("http://localhost:5000/api/User/signup", content);

            if (response.IsSuccessStatusCode)
            {
                // On success, redirect to the Login page
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // If signup fails, add a model error and return to the sign-up form
                ModelState.AddModelError(string.Empty, "Signup failed. Please try again.");
                return View(model);
            }
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginModel();
            return View(model); // Return the login view
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
    
            // Serialize the model to JSON
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Call the API to log in
            var response = await _httpClient.PostAsync("http://localhost:5000/api/User/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response to get userId and token
                var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(responseContent); // Define LoginResponseDto class

                // Store the token and userId in session
                HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                HttpContext.Session.SetInt32("UserId", loginResponse.UserId);
                return RedirectToAction("Index", "Task");

            }
            else
            {
                // Get the error message from the API response if available
                var errorMessage = "Login failed. Please check your credentials and try again.";
                if (response.Content != null)
                {
                    var apiError = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(apiError))
                    {
                        errorMessage = apiError; // Use the API-provided error message
                    }
                }

                // Add the error message to the ModelState
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }
        }
        [HttpPost]

        public IActionResult LogOut()
        {
            // Clear the user's session
            HttpContext.Session.Clear(); // Or use any other method depending on how you store the user's session or authentication
          
            return RedirectToAction("Login", "Account"); // Redirect to the login page
        }
    }
}
