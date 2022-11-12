using Microsoft.EntityFrameworkCore;
using WebApplication.Chat.Database.Entities;

namespace WebApplication.Chat.Database;

public class ChatDataContext : DbContext
{
	public ChatDataContext(DbContextOptions dbContextOptions)
		: base(dbContextOptions)
	{

	}

	public virtual DbSet<UserEntity> Users { get; set; } = null!;//virtual потому что так надо --_--
	public virtual DbSet<MessageEntity> Messages { get; set; } = null!;
	public virtual DbSet<GroupEntity> Groups { get; set; } = null!;
	public virtual DbSet<LogEventsEntity> LogEvents { get; set; } = null!;
	public virtual DbSet<GroupStatesEntity> GroupStates { get; set; } = null!;
	public virtual DbSet<BanEntity> BanList { get; set; } = null!;
}