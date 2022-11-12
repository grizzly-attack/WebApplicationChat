using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class AddBanModel
    {
        [Required] public long UserId { get; set; }
        [Required] public long GroupId { get; set; }
        [Required] public string Reason { get; set; } = string.Empty;
    }
}
