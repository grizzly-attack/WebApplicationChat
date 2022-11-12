using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class LeaveGroupModel
    {
        [Required] public long GroupId { get; set; }
    }
}
