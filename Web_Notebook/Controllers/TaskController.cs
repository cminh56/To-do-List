using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Web_Notebook.Models.Task;
using Microsoft.AspNetCore.Http;
using Web_Notebook.Models;
using Web_Notebook.Helpers;  // To use session storage for userId

public class TaskController : Controller
{
    private readonly HttpClient _httpClient;

    // Inject HttpClient
    public TaskController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }



    // GET: Task List (No need for Route, use default routing)
    public async Task<IActionResult> Index()
    {
        // Retrieve userId from session or authentication context (e.g., logged in user)
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            // Handle case when userId is not found (user is not logged in or session expired)
            return RedirectToAction("Login", "Account");
        }

        // Call the API to get tasks for the specific userId
        var response = await _httpClient.GetAsync($"http://localhost:5000/api/Task/user/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var tasks = JsonConvert.DeserializeObject<List<TaskDTO>>(content);  // TaskDTO is your data model
           
            CheckAndAddNotifications(tasks);

            // Pass tasks to the view
            return View(tasks);
        }
        else
        {
            // Handle error, for example, by displaying an error message
            return View("Error", new { message = "Failed to load tasks" });
        }


    }

    public void CheckAndAddNotifications(List<TaskDTO> tasks)
    {
        var notifications = new List<Notification>();

        foreach (var task in tasks)
        {
            // If DueDate is nullable, we need to check if it has a value
            if (task.DueDate.HasValue && (task.DueDate.Value - DateTime.Now).TotalHours <= 24)
            {
                notifications.Add(new Notification
                {
                    Message = $"Task {task.TaskId} is due tomorrow!",
                    DueDate = task.DueDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsRead = false
                });
            }
        }

        HttpContext.Session.SetObjectAsJson("Notifications", notifications);
    }



 
    public IActionResult Create()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Initialize the model with UserId from session
        var model = new CreateTaskDTO
        {
            UserId = userId.Value,
            UpdatedAt = DateTime.UtcNow
        };

        return View(model);
    }

    // POST: Task/Create

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDTO taskDTO)
    {
        if (!ModelState.IsValid)
        {
            return View(taskDTO); // return back with validation errors if any
        }

        // Create the task by calling the API
        var jsonContent = JsonConvert.SerializeObject(taskDTO);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:5000/api/Task", content);

        if (response.IsSuccessStatusCode)
        {
            // Redirect to Task list after successfully creating the task
            return RedirectToAction("Index", "Task");
        }
        else
        {
            // Handle error (e.g., show an error page or message)
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", errorMessage);
            return View(taskDTO);
        }


    }

    // Edit GET method: Fetch task details for editing
    public async Task<IActionResult> Edit(int taskId)
    {
        // Replace with your API call logic
        var response = await _httpClient.GetAsync($"http://localhost:5000/api/task/{taskId}");
        if (!response.IsSuccessStatusCode)
        {
            // Handle error
            return NotFound();
        }

        var taskJson = await response.Content.ReadAsStringAsync();
        var task = JsonConvert.DeserializeObject<TaskDTO>(taskJson);

        return View(task);
    }

    // Edit POST method: Save edited task
    [HttpPost]
    public async Task<IActionResult> Edit( TaskDTO taskDTO)
    {
      

        // Send PUT request to update task
        var taskData = new StringContent(JsonConvert.SerializeObject(taskDTO), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"http://localhost:5000/api/task/{taskDTO.TaskId}", taskData);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Task"); // Redirect after successful update
        }
        else
        {
            // Handle error on save
            ViewBag.ErrorMessage = "Error updating the task.";
            return View(taskDTO);
        }
    }

  
}