using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Web_Notebook.Models.Note;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Web_Notebook.Models.Task;
using System.Data;

public class NoteController : Controller
{
    private readonly HttpClient _httpClient;

    // Inject HttpClient
    public NoteController(HttpClient httpClient)
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
            notes= notes.Where(n => n.Status== "Active").ToList();
            return View(notes); // Pass the list of notes to the view
        }
        else
        {
            // Handle failure
            ViewBag.ErrorMessage = "Error retrieving notes from the server.";
            return View("Error");
        }
    }

    // GET: /Note/Create
    public IActionResult Create()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Initialize the model with UserId from session
        var model = new NoteDTO
        {
            UserId = userId.Value,
           
        };

        return View(model);
    }

    // Method to call the AddNote API using HttpClient
    [HttpPost]
    public async Task<IActionResult> Create(NoteDTO noteDTO)
    {
        // API URL
        var apiUrl = "http://localhost:5000/api/note"; // Use your correct API URL

        // Convert the NoteDTO to JSON
        var content = new StringContent(
            JsonConvert.SerializeObject(noteDTO),
            Encoding.UTF8,
            "application/json"
        );

        // Make the POST request to the AddNote API
        var response = await _httpClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            // Successfully created the note, process the response
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdNote = JsonConvert.DeserializeObject<NoteDTO>(responseContent);

            // Optionally, return the response or redirect as necessary
            return RedirectToAction("Index"); // Redirect to your note list page after successful creation
        }
        else
        {
            // Handle failed request
            ViewBag.ErrorMessage = "Error while creating note!";
            return View("Error");
        }
    }


    }
