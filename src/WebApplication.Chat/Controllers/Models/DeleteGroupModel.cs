using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Controllers.Models
{
    public class DeleteGroupModel
    {
        [Required] public long Id { get; set; }
    }
}
