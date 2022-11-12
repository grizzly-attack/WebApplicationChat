using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class CreateMessageModel
    {
        [Required] public long RecipientId { get; set; }
        [Required] public string Message { get; set; } = string.Empty;

    }
}
