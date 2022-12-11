using System.ComponentModel.DataAnnotations;
using System.IO;
using ChessGame.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ChessGame.Pages
{
    
    public class RegisterModel : PageModel
    {
        private DataBaseController _controller;
        
        private readonly ILogger<RegisterModel> _logger;
        [BindProperty, Required, MinLength(2, ErrorMessage = "Must be at least 2-character long.")]  
        [RegularExpression(@"[\S]+$", ErrorMessage = "Should not contains spaces.")] public string Username { get; set; }
        [BindProperty, Required, MinLength(6, ErrorMessage = "Must be at least 6-character long.")] 
        [RegularExpression(@"[\S]+$", ErrorMessage = "Should not contains spaces.")] public string Password { get; set; }
        [BindProperty, Required, Compare("Password", ErrorMessage = "Confirm password doesn't match.")] public string Confirm { get; set; }

        public RegisterModel(ILogger<RegisterModel> logger)
        {
            _logger = logger;
            Username = "";
            Password = "";
            Confirm = "";
            _controller = new DataBaseController();
        }

        public void OnGet()
        {
            GlobalOptions.IsLoggedIn = false;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (!_controller.RegisterUser(Username, Password)) return Page();
            GlobalOptions.IsLoggedIn = true;
            GlobalOptions.Username = Username;
            return RedirectToPage("/Menu");
        }
    }
}