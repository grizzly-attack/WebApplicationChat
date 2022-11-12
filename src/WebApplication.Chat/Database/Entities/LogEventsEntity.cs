using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Database.Entities
{
    public class LogEventsEntity
    {
        [Key] public long Id { get; set; }
        public long UserId { get; set; }
        public string IP { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public string Action { get; set; } = string.Empty;
    }
}
