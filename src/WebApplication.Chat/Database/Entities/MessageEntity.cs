using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Database.Entities;

public class MessageEntity
{
	[Key] public long Id { get; set; }
	public string TextMessage { get; set; } = string.Empty;
	public long UserId { get; set; }
	public long ReplayUserId { get; set; }
	public DateTime Created { get; set; }
	public DateTime Deleted { get; set; }
	public long GroupId { get; set; }

}