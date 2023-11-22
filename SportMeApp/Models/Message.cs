using System.ComponentModel.DataAnnotations;

namespace SportMeApp.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
