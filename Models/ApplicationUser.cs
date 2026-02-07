using Microsoft.AspNetCore.Identity;

namespace FreelancerPortfolioManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        // optional extra profile fields
        public string? DisplayName { get; set; }
    }
}