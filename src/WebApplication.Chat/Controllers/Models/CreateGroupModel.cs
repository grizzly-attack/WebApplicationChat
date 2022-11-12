using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class CreateGroupModel
    {
        [Required] public string Name { get; set; } = string.Empty;
    }
}
