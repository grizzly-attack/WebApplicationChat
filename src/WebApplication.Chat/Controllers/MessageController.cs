using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;//библиотка для обработки веб-запросов
using WebApplication.Chat.Controllers.Models;
using WebApplication.Chat.Database;
using WebApplication.Chat.Database.Entities;


namespace WebApplication.Chat.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class MessageController : ControllerBase
    {
        private readonly ChatDataContext _chatDataContext;

        public MessageController(ILogger<MessageController> logger,
                              ChatDataContext chatDataContext)
        {
            _chatDataContext = chatDataContext;
        }

        private long GetUserId()
        {
            return Convert.ToInt64(HttpContext.User.FindFirst("userid")!.Value);
        }

        [HttpGet]
        public ActionResult GetMessages()
        {
            return Ok(_chatDataContext.Messages.ToArray());
        }


        [HttpPut]
        public ActionResult CreateMessage([FromBody] CreateMessageModel model)
        {
            var userId = GetUserId();
            var group = _chatDataContext.Groups.FirstOrDefault(g => g.Id == model.RecipientId);
            if (group == null) 
                return BadRequest($"Группа {model.RecipientId} не найдена");

            var groupState = _chatDataContext.GroupStates.FirstOrDefault(g => g.GroupId == model.RecipientId
                                                                              && g.UserId == userId);
            if (groupState == null) 
                return BadRequest($"Группа {model.RecipientId} не найдена для пользователя {userId}");

            var messageEntity = new MessageEntity
            {
                TextMessage = model.Message,
                UserId = userId,
                Created = DateTime.UtcNow,
                GroupId = model.RecipientId,
            };
            _chatDataContext.Messages.Add(messageEntity);
            _chatDataContext.SaveChanges();

            groupState.MessageId = messageEntity.Id;
            _chatDataContext.SaveChanges();

            return Ok(messageEntity);
        }

        [HttpGet]
        public ActionResult GetLatestMessages()
        {
            var userId = GetUserId();
            var groupStates = _chatDataContext.GroupStates
                .Where(g => g.UserId == userId)
                .ToList();

            var result = new Dictionary <long , long>();

            foreach (var groupState in groupStates)
            {
                var messageCount = _chatDataContext.Messages
                    .Where(m => m.GroupId == groupState.GroupId && m.Id > groupState.MessageId)
                    .Count();

                result.Add(groupState.GroupId, messageCount);
            }

            return Ok(result);
        }

        [HttpGet]
        public ActionResult GetLatestMessagesFromGroup(int groupId) 
        {
            var userId = GetUserId();
            var groupState = _chatDataContext.GroupStates.FirstOrDefault(g => g.UserId == userId && g.GroupId == groupId);
            if (groupState == null) 
                return BadRequest("Группа не найдена");

            var messages = _chatDataContext.Messages
                .Where(m => m.GroupId == groupState.GroupId && m.Id > groupState.MessageId)
                .ToList();

            return Ok(messages);
        }
    }
}