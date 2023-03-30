using Microsoft.AspNetCore.Identity;

namespace MasterDetail.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = default!;
    }
}