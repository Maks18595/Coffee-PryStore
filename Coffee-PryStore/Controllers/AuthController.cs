using Coffee_PryStore.Models;
using Coffee_PryStore.Models.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Coffee_PryStore.Controllers
{
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly DataBaseHome _dataBaseHome; // Ваш контекст бази даних

        public AuthController(TokenService tokenService, DataBaseHome dataBaseHome)
        {
            _tokenService = tokenService;
       
            _dataBaseHome = dataBaseHome;
        }
        
        public class UserLoginDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public void RegisterUser(Models.User user, string password)
        {
            var passwordHasher = new PasswordHasher<Models.User>();
            user.Password = passwordHasher.HashPassword(user, password);
            _dataBaseHome.Users.Add(user);
            _dataBaseHome.SaveChanges();
        }

        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = _dataBaseHome.Users.FirstOrDefault(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized(); // Якщо користувача не знайдено
            }

            var passwordHasher = new PasswordHasher<Models.User>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(); // Якщо пароль не співпадає
            }

            // Створення claims для користувача
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, "Admin") // Додайте роль тут, якщо це адміністратор
    };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Вхід користувача
            await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }
    }
}