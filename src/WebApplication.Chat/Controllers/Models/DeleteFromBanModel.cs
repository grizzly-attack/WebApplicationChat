using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class DeleteFromBanModel
    {
        [Required] public long UserId { get; set; }
        [Required] public long GroupId { get; set; }
    }
}
