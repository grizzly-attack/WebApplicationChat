using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Database.Entities
{
    public class GroupStatesEntity
    {
        [Key] public long Id { get; set; }
        public long GroupId { get; set; }
        public long UserId { get; set; }
        public long MessageId { get; set; }
    }
}
