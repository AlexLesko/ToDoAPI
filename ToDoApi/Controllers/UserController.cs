using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using ToDoApi.Service;

namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly LoggerService loggerService;
        public UserController(DataContext context)
        {
            _context = context;
            loggerService = new LoggerService(context);
        }

        [HttpGet("Logs")]
        public async Task<ActionResult<List<LoggerModel>>> GetLogs()
        {
            return Ok(await _context.loggerDb.ToListAsync());
        }

        [HttpGet]
        public async Task<ActionResult<List<Users>>> GetUsers()
        {
            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpPost("Filtered")]
        public async Task<ActionResult<List<Users>>> GetFilteresUsers(Users user)
        {
            return Ok(await GetFilteredUsers(user));
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<List<Users>>> CreateUser(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpPost("AddTask/{userName}")]
        public async Task<ActionResult<List<Users>>> AddTask(string userName, ToDoModel task)
        {
            Users loggedUser = await _context.Users.FirstOrDefaultAsync(x => x.Name == userName);

            _context.ToDoModel.Add(task);
            await _context.SaveChangesAsync();

            await loggerService.UpdateLogDb(userName, task.Name, "Add Task for userID:" + task.UsersId);

            return Ok(await GetFilteredUsers(loggedUser));
        }

        [HttpPut("{userName}")]
        public async Task<ActionResult<List<Users>>> UpdateUser(string userName, Users user)
        {
            Users loggedUser = await _context.Users.FirstOrDefaultAsync(userDB => userDB.Id == user.Id);
            var dbUser = await _context.Users.Include(e => e.ToDoList).FirstOrDefaultAsync(x => x.Id == user.Id);
            if (dbUser == null)
            {
                return BadRequest("User not found!");
            }

            dbUser.Name = user.Name;
            dbUser.UserRight = user.UserRight;
            dbUser.ToDoList = user.ToDoList;
            dbUser.ImageTitle = user.ImageTitle;
            dbUser.ImageData = user.ImageData;

            var dbAuthUser = await _context.userAuths.FirstOrDefaultAsync(userAuth => userAuth.UserId == user.Id);
            dbAuthUser.Name = user.Name;

            await _context.SaveChangesAsync();

            return Ok(await GetFilteredUsers(loggedUser));
        }

        [HttpPut("UpdateTask/{userName}")]
        public async Task<ActionResult<List<Users>>> UpdateTask(string userName, ToDoModel task)
        {
            Users loggedUser = await _context.Users.FirstOrDefaultAsync(x => x.Name == userName);
            var dbTask = await _context.ToDoModel.FindAsync(task.Id);
            if (dbTask == null)
            {
                return BadRequest("Task not found!");
            }

            dbTask.Name = task.Name;
            dbTask.Date = task.Date;

            await _context.SaveChangesAsync();

            await loggerService.UpdateLogDb(userName, task.Name, "Update Task for userID:" + task.UsersId);

            return Ok(await GetFilteredUsers(loggedUser));
        }

        [HttpDelete("{userName}/{id}")]
        public async Task<ActionResult<List<Users>>> DeleteUser(string userName,int id)
        {
            Users loggedUser = await _context.Users.FirstOrDefaultAsync(x => x.Name == userName);
            var dbUser = await _context.Users.Include(e => e.ToDoList).FirstOrDefaultAsync(x => x.Id == id);
            if (dbUser == null)
            {
                return BadRequest("User not found!");
            }

            var tasks = _context.ToDoModel.Where(y => y.UsersId == id);

            foreach(var task in tasks)
            {
                _context.ToDoModel.Remove(task);
            }

            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();

            UserAuth userToRemove = await _context.userAuths.FirstOrDefaultAsync(user => user.UserId == id);
            _context.userAuths.Remove(userToRemove);
            await _context.SaveChangesAsync();

            if (dbUser.Name == userName)
            {
                return Ok(null);
            }
            else
            {
                return Ok(await GetFilteredUsers(loggedUser));
            }
        }

        [HttpDelete("DeleteTask/{userName}/{id}")]
        public async Task<ActionResult<List<Users>>> DeleteTask(string userName, int id)
        {
            Users loggedUser = await _context.Users.FirstOrDefaultAsync(x => x.Name == userName);
            var dbTask = await _context.ToDoModel.FindAsync(id);
            if(dbTask == null)
            {
                return BadRequest("Task not Found!");
            }

            _context.ToDoModel.Remove(dbTask);
            await _context.SaveChangesAsync();

            await loggerService.UpdateLogDb(userName, dbTask.Name, "Deleted Task for userID:" + dbTask.UsersId);

            return await GetFilteredUsers(loggedUser);
        }

        private async Task<List<Users>> GetFilteredUsers(Users user)
        {
            if (user.UserRight == "Admin")
            {
                return await _context.Users.Include(e => e.ToDoList).ToListAsync();
            }
            else
            {
                return await _context.Users.Include(e => e.ToDoList).Where(x => x.Name == user.Name).ToListAsync();
            }
        }
    }
}
