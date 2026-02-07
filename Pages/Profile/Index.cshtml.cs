using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreelancerPortfolioManager.Models;

namespace FreelancerPortfolioManager.Pages.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Read-only properties for display
        public string? DisplayName { get; private set; }
        public string? Email { get; private set; }
        public string? PhoneNumber { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            DisplayName = user.DisplayName;
            Email = user.Email;
            PhoneNumber = await _userManager.GetPhoneNumberAsync(user);

            return Page();
        }
    }
}
