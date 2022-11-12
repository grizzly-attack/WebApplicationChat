using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class JoinToGroupModel
    {
        [Required] public long GroupId { get; set; }
    }
}
