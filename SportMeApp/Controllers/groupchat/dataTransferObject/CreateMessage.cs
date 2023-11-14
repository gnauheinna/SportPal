using SportMeApp.Models;

namespace SportMeApp.Controllers.groupchat.dataTransferObject
{
    public class CreateMessage
    {
        public string Text { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
}
