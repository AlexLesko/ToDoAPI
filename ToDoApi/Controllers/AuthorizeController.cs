using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.Service;

namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly AuthService authService;
        public AuthorizeController(DataContext context)
        {
            _context = context;
            authService = new AuthService();
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Users>> RegisterUser(UserRegister user)
        {
            UserAuth userAuth = new UserAuth();
            
            byte[] passwordHash;
            byte[] passwordSalt;

            var dbUser = await _context.Users.FirstOrDefaultAsync(dbUser => dbUser.Name == user.users.Name);
            if(dbUser != null)
            {
                return BadRequest("User already exist!");
            }

            authService.CreatePasswordHash(user.userRequest.password, out passwordHash, out passwordSalt);
            
            userAuth.Name = user.userRequest.userName;
            userAuth.UserId = user.users.Id;
            userAuth.PasswordHash = passwordHash;
            userAuth.PasswordSalt = passwordSalt;
            
            _context.userAuths.Add(userAuth);
            _context.Users.Add(user.users);
            await _context.SaveChangesAsync();

            return Ok(user.users);
        }
    }
}
