using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web_Notebook.Models.Note;

namespace Web_Notebook.Controllers
{
    public class BinController : Controller
    {
        private readonly HttpClient _httpClient;

        // Inject HttpClient
        public BinController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Action to get the list of notes by userId
        public async Task<IActionResult> Index()
        {

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Handle case when userId is not found (user is not logged in or session expired)
                return RedirectToAction("Login", "Account");
            }
            var apiUrl = $"http://localhost:5000/api/Note/user/{userId}";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var notes = JsonConvert.DeserializeObject<List<Note>>(jsonResponse); // Assuming Note is the class for your note data
                notes = notes.Where(n => n.Status == "Inactive").ToList();
                return View(notes); // Pass the list of notes to the view
            }
            else
            {
                // Handle failure
                ViewBag.ErrorMessage = "Error retrieving notes from the server.";
                return View("Error");
            }
        }

    }
}
