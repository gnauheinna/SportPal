﻿
using SportMeApp.Models;

namespace SportMeApp.Controllers.GroupChat.dataTransferObject
{
    public class CreateMessage
    {
        public string Text { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
    }
}
