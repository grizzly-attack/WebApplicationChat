using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Chat.Database.Entities;

[Index(nameof(Login), IsUnique = true, Name = "ix_login")]
public class UserEntity
{
	[Key] public long Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Login { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public DateTime LastEnter { get; set; }
}