using Coffee_PryStore.Models;
using Coffee_PryStore.Models.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace Coffee_PryStore.Controllers
{
    public class AuthController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly DataBaseHome _context; // Ваш контекст бази даних

        public AuthController(TokenService tokenService, DataBaseHome context)
        {
            _tokenService = tokenService;
            _context = context;
        }
        public class UserLoginDto
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
       [HttpPost]
        public IActionResult Login(UserLoginDto loginDto)
        {
            // Логіка перевірки користувача
            var user = _context.Users.FirstOrDefault(u => u.UserName == loginDto.UserName && u.Password == loginDto.Password); // Змінити на правильну логіку

            if (user == null)
            {
                return Unauthorized(); // Повертаємо 401, якщо користувача не знайдено
            }

            var token = _tokenService.GenerateToken(user);


            return Ok(new { Token = token });
        }
    }
}