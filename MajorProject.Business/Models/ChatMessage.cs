using System;

namespace MajorProject.Business.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime CreatedAt { get; set; }

        public ChatMessage()
        {
            Message = string.Empty;
            Sender = string.Empty;
            CreatedAt = DateTime.Now;
        }
    }
}
