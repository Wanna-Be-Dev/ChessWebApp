using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ChessGame.Pages
{
    public class Menu : PageModel
    {
        private readonly ILogger<Menu> _logger;

        [BindProperty, Required(ErrorMessage = "You must choose a type of game.")]
        public string GameType { get; set; }
        
        public Menu(ILogger<Menu> logger)
        {
            _logger = logger;
        }
    
        public void OnGet()
        {
            
        }

        public ActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            GlobalOptions.GameType = GameType switch
            {
                "Player vs Player" => ChessGame.GameType.PlayervsPlayer,
                "Player vs AI" => ChessGame.GameType.PlayervsAI,
                "AI vs AI" => ChessGame.GameType.AIvsAI,
                _ => GlobalOptions.GameType
            };
            return RedirectToPage("/Chess");
        }
    }
}