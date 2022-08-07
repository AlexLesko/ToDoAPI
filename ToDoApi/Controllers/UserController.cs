using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;


namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Users>>> GetUsers()
        {
            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpPost("Filtered")]
        public async Task<ActionResult<List<Users>>> GetFilteresUsers(Users user)
        {
            if(user.UserRight == "Admin")
            {
                return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
            }
            else
            {
                return Ok(_context.Users.Include(e => e.ToDoList).Where(x => x.Name == user.Name));//. .FirstOrDefaultAsync(x => x.Name == user.Name));
            }
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<List<Users>>> CreateUser(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpPost("AddTask")]
        public async Task<ActionResult<Users>> AddTask(ToDoModel task)
        {
            _context.ToDoModel.Add(task);
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<Users>>> UpdateUser(Users user)
        {
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

            await _context.SaveChangesAsync();

            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpPut("UpdateTask")]
        public async Task<ActionResult<Users>> UpdateTask(ToDoModel task)
        {
            var dbTask = await _context.ToDoModel.FindAsync(task.Id);
            if (dbTask == null)
            {
                return BadRequest("Task not found!");
            }

            dbTask.Name = task.Name;
            dbTask.Date = task.Date;

            await _context.SaveChangesAsync();

            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Users>>> DeleteUser(int id)
        {
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

            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }

        [HttpDelete("DeleteTask/{id}")]
        public async Task<ActionResult<Users>> DeleteTask(int id)
        {
            var dbTask = await _context.ToDoModel.FindAsync(id);
            if(dbTask == null)
            {
                return BadRequest("Task not Found!");
            }

            int userId = dbTask.UsersId;
            _context.ToDoModel.Remove(dbTask);
            await _context.SaveChangesAsync();

            return Ok(await _context.Users.Include(e => e.ToDoList).ToListAsync());
        }
    }
}
