using ChessGame.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChessGame.Pages
{
    public class LoginModel : PageModel
    {
        private DataBaseController _controller;
        public LoginModel()
        {
            Username = "";
            Password = "";
            _controller = new DataBaseController();
        }

        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }

        public void OnGet()
        {
            GlobalOptions.IsLoggedIn = false;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (!_controller.LogInUser(Username, Password)) return Page();
            GlobalOptions.IsLoggedIn = true;
            GlobalOptions.Username = Username;
            return RedirectToPage("/Menu");
        }
    }
}