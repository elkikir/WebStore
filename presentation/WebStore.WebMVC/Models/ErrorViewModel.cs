namespace WebStore.WebMVC.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
    }
}
