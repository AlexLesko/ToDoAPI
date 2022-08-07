namespace ToDoApi.Service
{
    public class LoggerService
    {
        private readonly DataContext _context;
        public LoggerService(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> UpdateLogDb(string user,string task, string action)
        {
            LoggerModel loggerModel = new LoggerModel();
            loggerModel.User = user;
            loggerModel.Task = task;
            loggerModel.Modified = DateTime.Now;
            loggerModel.Action = action;
            
            _context.loggerDb.Add(loggerModel);
            await _context.SaveChangesAsync();

            return null;
        }
    }
}
