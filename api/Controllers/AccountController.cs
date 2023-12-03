using System.Security.Cryptography;
using System.Text;
using api.Data;
using api.DTOs;
using api.Entities;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    //URI = localhost/api/Account/<Endpoint Name>
    public class AccountController : BaseApiController
    {
        //DbContext: The Entity framework database object which interacts with database
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(ApplicationDbContext context,ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }


        //URI = localhost/api/Account/Register (POST) , This wont return anything if we send a get request
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register){
         /*
            These are some validations which will check if the username email and phone are registered or not,
            if they are registered, then this will throw a Bad request with the custom error message 
        */
            if (await UserExists(register.Username))        return BadRequest("Username is taken");
            if (await EmailIsRegistered(register.Email))    return BadRequest("Email is taken");
            if (await PhoneIsRegistered(register.Phone))    return BadRequest("This Phone number is taken");

        /*
            If the User credentials pass the above validations then  
        */
            using HMACSHA512 hmac = new();
            AppUser user = new()
            {
                FullName = register.FullName.ToLower(),
                Username = register.Username.ToLower(),
                Email    = register.Email.ToLower(),
                Phone    = register.Phone,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO {
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
            
            AppUser user = await _context.Users.SingleOrDefaultAsync(x => x.Username == loginDTO.Username);
            if (user is null) return Unauthorized("User doesnt exist");

            HMACSHA512 key = new(user.PasswordSalt);
            byte[] comparedHash = key.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i = 0; i < comparedHash.Length;i++){
                if (comparedHash[i] != user.PasswordHash[i]) return Unauthorized("Password is Incorrect");
            }
            return new UserDTO {
                Username = user.Username,
                Token = _tokenService.CreateToken(user)
            };
        }
        
        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x => x.Username == username.ToLower());
        }
        private async Task<bool> EmailIsRegistered(string email){
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }
        private async Task<bool> PhoneIsRegistered(string phone){
            return await _context.Users.AnyAsync(x => x.Phone == phone);
        }
    }
}