using Core.DTOs.UserDtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AlturStudyCase.Controllers
{
    public class BaseController : Controller
    {
        protected AuthenticationDto CurrentUser
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return new AuthenticationDto
                    {
                        Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                        FirstName = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
                        LastName = User.FindFirst(ClaimTypes.Surname)?.Value ?? "",
                        Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                        Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Employee",
                        Username = User.FindFirst(ClaimTypes.Email)?.Value?.Split('@')[0] ?? ""
                    };
                }
                return null;
            }
        }

        protected bool IsManager => CurrentUser?.Role == "Manager";
        protected bool IsEmployee => CurrentUser?.Role == "Employee";
        protected string UserEmail => CurrentUser?.Email ?? "";
        protected string UserName => CurrentUser?.Username ?? "";
        protected int UserId => CurrentUser?.Id ?? 0;
    }
}
