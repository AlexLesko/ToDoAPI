namespace ToDoApi.Model
{
    public class LoggerModel
    {
        public int Id { get; set; }
        public string User { get; set; } = string.Empty;
        public string Task { get; set; } = string.Empty;
        public DateTime Modified { get; set; }
        public string Action { get; set; } = string.Empty;
    }
}
