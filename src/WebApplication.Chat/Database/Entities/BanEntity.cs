using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Database.Entities
{
    public class BanEntity
    {
        [Key] public long Id { get; set; }
        public long UserId { get; set; }
        public long GroupId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public long AdminId { get; set; }

    }
}
