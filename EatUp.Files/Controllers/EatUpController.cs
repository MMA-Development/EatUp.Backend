using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Files.Controllers
{
    public abstract class EatUpController: Controller
    {
        private string _role => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        public string? VendorId => _role == "Vendor" ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        public string? UserId => _role == "User" ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
    }
}
