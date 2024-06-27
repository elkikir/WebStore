namespace WebStore.WebMVC.Models
{
    public class ConfirmationModel
    {
        public int OrderId { get; set; }
        public string CellPhone { get; set; }

        public Dictionary<string, string> Errors = new Dictionary<string, string>();
    }
}
