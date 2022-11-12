using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Database.Entities
{
    public class GroupEntity
    {
        [Key] public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long UserId { get; set; }
        public string GroupType { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
    }
}
