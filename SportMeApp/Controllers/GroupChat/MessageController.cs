// SendMesasge, GetMessages,

using SportMeApp.Controllers.GroupChat.dataTransferObject;
using SportMeApp.Models;
using Microsoft.AspNetCore.Mvc;
using PusherServer;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;

namespace SportMeApp.Controllers
{
    [ApiController]
    [Route("api/group/")]
    public class MessageController : ControllerBase
    {
        private readonly SportMeContext _context;
        private readonly Pusher _pusher;

        public MessageController(Pusher pusher, SportMeContext context)
        {
            _context = context;
            _pusher = pusher;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessage Partialmessage)
        {


            if (Partialmessage == null || Partialmessage.UserId == 0 || Partialmessage.EventId == 0 || string.IsNullOrEmpty(Partialmessage.Text))
            {
                return BadRequest("Invalid message data");
            }

            var message = new Message
            {
                Text = Partialmessage.Text,
                Timestamp = DateTime.UtcNow,
                UserId = Partialmessage.UserId,
                EventId = Partialmessage.EventId

            };

            // save messages to db and call pusher to send message to users that subscribed to the Event chat
            _context.Message.Add(message); // add message to database
            await _context.SaveChangesAsync(); // save changes

    
            
            var message_detail = await _context.Message
                .Include(m => m.User) // Load the User navigation property
                .Where(m => m.MessageId == message.MessageId) // Filter by the newly added message
                .Select(m => new
                {
                    MessageId = m.MessageId,
                    UserId = m.UserId,
                    UserName = m.User != null ? m.User.Username : null, // Check for null before accessing Username
                    Timestamp = m.Timestamp,
                    Text = m.Text
                })
                .FirstOrDefaultAsync();

            // Notify other clients using Pusher
            string channelId = "group_chat_" + message.EventId.ToString();
            await _pusher.TriggerAsync(channelId, "new_message", message_detail);
            return Ok(message_detail);
        }

        [HttpGet("{EventId}/GetMessages")]
        public IActionResult GetMessages(int EventId)
        {
            var messages = _context.Message
                .Where(m => m.EventId == EventId)
                .Select(m => new
                {
                    MessageId = m.MessageId,
                    UserId = m.UserId,
                    UserName = m.User.Username,
                    Timestamp = m.Timestamp,
                    Text = m.Text

                })
                .ToList();

            return Ok(messages);
        }
    }
}
