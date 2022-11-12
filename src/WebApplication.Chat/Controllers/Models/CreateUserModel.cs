using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class CreateUserModel
    {
        [Required] public string Login { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required] public string Name { get; set; } = string.Empty;
    }
}
