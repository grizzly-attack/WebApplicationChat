using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Chat.Controllers.Models;
using WebApplication.Chat.Database;
using WebApplication.Chat.Database.Entities;

namespace WebApplication.Chat.Controllers
{
    [Authorize]
    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ChatDataContext _chatDataContext;

        public GroupController(ILogger<GroupController> logger,
                              ChatDataContext chatDataContext)
        {
            _chatDataContext = chatDataContext;
        }

        private long GetUserId ()
        {
            var user = HttpContext.User.FindFirst("userid");
            if (user == null)
                throw new Exception ("Пользователь не найден в токене");

            return Convert.ToInt64(user.Value);
        }

        [HttpGet]
        public ActionResult GetUserGroups ()
        {
            var userId = GetUserId();
            var userGroups = _chatDataContext.Groups.Where(g => g.UserId == userId).ToList();
            return Ok(userGroups);
        }

        [HttpPut]
        public ActionResult CreateGroup ([FromBody] CreateGroupModel model)
        {
            var group = new GroupEntity
            {
                Name = model.Name,
                UserId = GetUserId(),
                GroupType = "common",
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
            };

            _chatDataContext.Groups.Add(group);
            _chatDataContext.SaveChanges();

            var groupStates = new GroupStatesEntity
            {
                UserId = GetUserId(),
                MessageId = 0,
                GroupId = group.Id,
            };

            _chatDataContext.GroupStates.Add(groupStates);
            _chatDataContext.SaveChanges();

            return Ok(group);
        }

        [HttpDelete]
        public ActionResult DeleteGroup([FromBody] DeleteGroupModel model)
        {
            var userId = GetUserId();
            var group = _chatDataContext.Groups.FirstOrDefault(g => g.Id == model.Id && g.UserId == userId);
            if (group == null) 
                return BadRequest("Группа не найдена");

            _chatDataContext.Groups.Remove(group);
            _chatDataContext.SaveChanges();

            var groupStates = _chatDataContext.GroupStates.Where(g => g.GroupId == model.Id).ToArray();
            _chatDataContext.GroupStates.RemoveRange(groupStates);
            _chatDataContext.SaveChanges();

            var messages = _chatDataContext.Messages.Where(g => g.GroupId == model.Id).ToArray();
            _chatDataContext.Messages.RemoveRange(messages);
            _chatDataContext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public ActionResult JoinToGroup([FromBody] JoinToGroupModel model)
        {
            var userId = GetUserId();
            var group = _chatDataContext.Groups.FirstOrDefault(g => g.Id == model.GroupId);
            if (group == null) 
                return BadRequest("Группа не найдена");

            var groupState = _chatDataContext.GroupStates.FirstOrDefault(g => g.GroupId == model.GroupId
                                                                              && g.UserId == userId);
            if (groupState != null) 
                return BadRequest($"Группа {model.GroupId} уже имеется для пользователя {userId}");

            var banList = _chatDataContext.BanList.FirstOrDefault(b => b.UserId == userId && b.GroupId == model.GroupId);
            if (banList != null) 
                return BadRequest("Пользователь забанен");

            var groupStates = new GroupStatesEntity
            {
                UserId = GetUserId(),
                MessageId = 0,
                GroupId = group.Id,
            };

            _chatDataContext.GroupStates.Add(groupStates);
            _chatDataContext.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        public ActionResult LeaveGroup([FromBody] LeaveGroupModel model)
        {
            var userId = GetUserId();
            var groupState = _chatDataContext.GroupStates.FirstOrDefault(g => g.GroupId == model.GroupId
                                                                              && g.UserId == userId);
            if (groupState == null) 
                return BadRequest($"Группа {model.GroupId} не найдена");

            _chatDataContext.GroupStates.Remove(groupState);
            _chatDataContext.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public ActionResult AddBanList([FromBody] AddBanModel model)
        {
            var adminId = GetUserId();
            if (adminId == model.UserId) 
                return BadRequest("Себя забанить нельзя");

            var user = _chatDataContext.Users.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null) 
                return BadRequest("Пользователь не найден");

            var group = _chatDataContext.Groups.FirstOrDefault(g => g.Id == model.GroupId);
            if (group == null) 
                return BadRequest("Группа не найдена");

            if (adminId != group.UserId) 
                return BadRequest("Банить может только админ");
            
            var banList = _chatDataContext.BanList.FirstOrDefault(b => b.Id == model.UserId && b.GroupId == model.GroupId);
            if (banList != null) 
                return BadRequest("Пользователь уже в бан-листе");

            _chatDataContext.BanList.Add(new BanEntity { 
                AdminId = adminId,
                GroupId = model.GroupId,
                UserId = model.UserId,
                Reason = model.Reason,
            });

            var groupState = _chatDataContext.GroupStates.FirstOrDefault(u => u.UserId == model.UserId && u.GroupId == model.GroupId);
            if (groupState!= null)
            {
                _chatDataContext.GroupStates.Remove(groupState);
            }
            
            _chatDataContext.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        public ActionResult DeleteFromBanList([FromBody] DeleteFromBanModel model)
        {
            var adminId = GetUserId();

            var group = _chatDataContext.Groups.FirstOrDefault(g => g.Id == model.GroupId);
            if (group == null) 
                return BadRequest("Группа не найдена");

            if (adminId != group.UserId) 
                return BadRequest("Разбанить может только админ");

            var banList = _chatDataContext.BanList.FirstOrDefault(b => b.UserId == model.UserId && b.GroupId == model.GroupId);
            if (banList == null) 
                return BadRequest("Пользователь не найден");

            _chatDataContext.BanList.Remove(banList);
            _chatDataContext.SaveChanges();

            return Ok();
        }
    }
}