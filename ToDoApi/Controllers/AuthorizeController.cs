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

        [HttpPost("Login")]
        public async Task<ActionResult<Users>> LoginUser(UserRequest userRequest)
        {
            UserAuth userAuth = new UserAuth();

            var dbUser = await _context.Users.FirstOrDefaultAsync(user => user.Name == userRequest.userName);
            if(dbUser == null)
            {
                return BadRequest("User not found!");
            }

            var dbUserAuth = await _context.userAuths.FirstOrDefaultAsync(user => user.UserId == dbUser.Id);
            if(dbUserAuth != null)
            {
                userAuth = dbUserAuth;
                if(authService.VerifyPassword(userRequest.password, userAuth.PasswordHash, userAuth.PasswordSalt))
                {
                    return await _context.Users.FirstOrDefaultAsync(user => user.Id == userAuth.UserId);
                }
                else
                {
                    return BadRequest("Invalid password");
                }
            }
            else
            {
                return BadRequest("User or Password not found!");
            }
        }
    }
}
