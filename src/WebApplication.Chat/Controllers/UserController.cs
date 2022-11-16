using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Chat.Controllers.Models;
using WebApplication.Chat.Database;
using WebApplication.Chat.Database.Entities;

namespace WebApplication.Chat.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ChatDataContext _chatDataContext;

        public UserController(ILogger<UserController> logger,
                              ChatDataContext chatDataContext)
        {
            _chatDataContext = chatDataContext;
        }

        //[HttpGet]
        //[Authorize]
        //public ActionResult GetUsers()
        //{
        //    return Ok(_chatDataContext.Users.ToArray());
        //}

        [HttpGet]
        [Authorize]
        public ActionResult FindUser([Required] string name)
        {
            return Ok(_chatDataContext.Users
                .Where(u => u.Name == name)
                .ToArray());
        }

        [HttpPut]
        public ActionResult CreateUser([FromBody][Required] CreateUserModel model)
        {
            var sha256 = SHA256.Create();
            var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));
            var userEntity = new UserEntity { Name = model.Name, Login = model.Login, Password = passwordHash, LastEnter = DateTime.UtcNow };
            _chatDataContext.Users.Add(userEntity);
            _chatDataContext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public ActionResult Login([FromBody]LoginModel model)
        {
            var sha256 = SHA256.Create();
            var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password)));
            var user = _chatDataContext.Users.FirstOrDefault(u => u.Login == model.Login);
            if (user == null) 
                return BadRequest("Пользователь не найден");
            if (user.Password != passwordHash) 
                return BadRequest("Неверный пароль");


            user.LastEnter = DateTime.UtcNow;
            _chatDataContext.SaveChanges();

            var claims = new[]
           {
                new Claim("username", user.Name),
                new Claim("userid", user.Id.ToString())
            };

            var signingCredentials = 
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_server_long")), "HS256");
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = securityTokenHandler.CreateJwtSecurityToken("chat_server", "chat_server", 
                new ClaimsIdentity(claims), 
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(365.0),
                DateTime.UtcNow, signingCredentials);
            return Ok("Bearer " + securityTokenHandler.WriteToken(jwtSecurityToken));
        }
    }
}