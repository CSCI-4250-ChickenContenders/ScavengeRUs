using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScavengeRUs.Models.Entities;
using ScavengeRUs.Models.Enums;
using ScavengeRUs.Services;
using System;
using System.Security.Claims;
using System.IO;
using Microsoft.VisualBasic.FileIO;



namespace ScavengeRUs.Controllers
{

    /// <summary>
    /// Anything in this controller (www.localhost.com/users) can only be viewed by Admin
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly Functions _functions;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CodeGenerator _codeGenerator;
        string defaultPassword = "Etsupass12!";

        /// <summary>
        /// This is the dependecy injection for the User Repository that connects to the database
        /// </summary>
        /// <param name="userRepo"></param>
        public UserController(IUserRepository userRepo, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _userRepo = userRepo;
            _functions = new Functions(configuration);
            _userManager = userManager;
            _codeGenerator = new CodeGenerator(null);

        }
        /// <summary>
        /// This is the landing page for www.localhost.com/user/manage aka "Admin Portal"
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Manage(string searchString)
        {
            var users = await _userRepo.ReadAllAsync(); //Reads all the users in the db

            //if the admin didn't search for anything just return all the users
            if(string.IsNullOrEmpty(searchString))
                return View(users);  //Right click and go to view to see HTML

            //this line of code filters out all the users whose emails and phone numbers do not
            //contain the search string
            var searchResults = users.Where(user => user.Email.Contains(searchString) 
            || !string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumber.Contains(searchString));

            return View(searchResults);
        }
        /// <summary>
        /// This is the HtmlGet landing page for editing a User
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit([Bind(Prefix = "id")]string username)
        {
            //await _functions.SendEmail("4239006885@txt.att.net", "Hello, from ASP.NET", "Body");
            //await CreateUsers("testdata.csv");
            var user = await _userRepo.ReadAsync(username);
            return View(user);
        }
        /// <summary>
        /// This is the method that executes when hitting the submit button on a edit user form.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                await _userRepo.UpdateAsync(user.Id, user);
                return RedirectToAction("Manage");
            }
            return View(user);
        }
        /// <summary>
        /// This is the landing page to delete a user aka "Are you sure you want to delete user X?"
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete([Bind(Prefix ="id")]string username)
        {
            var user = await _userRepo.ReadAsync(username);
            if (user == null)
            {
                return RedirectToAction("Manage");
            }
            return View(user);
        }
        /// <summary>
        /// This is the method that gets executed with hitting submit on deleteing a user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// 
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([Bind(Prefix = "id")]string username)
        {
            await _userRepo.DeleteAsync(username);
            return RedirectToAction("Manage");
        }
        /// <summary>
        /// This is the landing page for viewing the details of a user (www.localhost.com/user/details/{username}
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details([Bind(Prefix = "id")]string username)
        {
            var user = await _userRepo.ReadAsync(username);

            return View(user);
        }
        /// <summary>
        /// This is the landing page to create a new user from the admin portal
        /// </summary>
        /// <returns></returns>

        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// This is the method that is executed when hitting submit on creating a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;
                await _userRepo.CreateAsync(user, defaultPassword);
                string teamCode = _codeGenerator.GenerateUniqueCode();

                return RedirectToAction("Details", new { id = user.UserName });
            }
            return View(user);
            
        }
        /// <summary>
        /// This is the profile page for a user /user/profile/
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userRepo.ReadAsync(User.Identity?.Name!);
            return View(currentUser);
        }

        public async Task<IActionResult> CreateUsers()
        {
            //filePath = "Services/Users.csv";
            //serverUrl = $"{Request.Scheme}://{Request.Host}";
            //await _userRepo.CreateUsers(filePath, serverUrl);

            return View();
        }

    /// <summary>
    /// This Task updates the team name for an application user using the
    /// provided team name, and in later iterations will write to this change
    /// to the database.
    /// </summary>
    /// <param name="teamName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UpdateTeamName(string teamName)
    {
        try
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the user exists
            if (currentUser == null)
            {
                return NotFound();
            }

            // Update the TeamName property
            currentUser.TeamName = teamName;

            // Update the user in the database
            await _userManager.UpdateAsync(currentUser);

            // Optionally, you can return a success message or redirect to a different page
            return Ok(new { Message = "Team name updated successfully." });
        }
        catch (Exception ex)
        {
            // Handle exceptions if needed
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating team name." });
        }
    }
    }
}
