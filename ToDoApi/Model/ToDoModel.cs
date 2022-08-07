namespace ToDoApi.Model
{
    public class ToDoModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int UsersId { get; set; }
    }
}
