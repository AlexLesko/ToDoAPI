namespace ToDoApi.Model
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserRight { get; set; } = string.Empty;
        public List<ToDoModel> ToDoList { get; set; } = new List<ToDoModel>();
        public string ImageTitle { get; set; } = string.Empty;
        public string ImageData { get; set; } = string.Empty;
    }
}
