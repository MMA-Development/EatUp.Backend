using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace EatUp.Meals.Controllers
{
    public abstract class EatUpController: Controller
    {
        private string _role => User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        public Guid? VendorId => _role == "Vendor" ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) : null;
        public Guid? UserId => _role == "User" ? Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) : null;
    }
}
