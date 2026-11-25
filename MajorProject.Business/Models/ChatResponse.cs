namespace MajorProject.Business.Models
{
    public class ChatResponse
    {
        public string Message { get; set; }
        public string Intent { get; set; }
        public bool Success { get; set; }
        public string ActionText { get; set; }
        public string ActionUrl { get; set; }

        public ChatResponse()
        {
            Message = string.Empty;
            Intent = string.Empty;
            Success = true;
            ActionText = string.Empty;
            ActionUrl = string.Empty;
        }
    }
}
