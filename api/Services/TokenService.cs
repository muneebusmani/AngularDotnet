using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Entities;
using api.Interfaces;
using Microsoft.IdentityModel.Tokens;
namespace api.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration cfg)
        {
            // Getting a value from the config files using cfg param and creating a security key out of it
            _key = new(Encoding.UTF8.GetBytes(cfg["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {

            // It is a piece of information to identify the user by a specified attribute, in this project that attribute is username of a user
            List<Claim> claims = new()
            {
                //                     Type             ,   Value
                new Claim(JwtRegisteredClaimNames.NameId,user.Username)
            };

            // Creation of Signature by which the tokens will be signed and encrypted
            SigningCredentials signature = new(

                // Security Key , Algorithm 
                _key            , SecurityAlgorithms.HmacSha512Signature);


            // Info used to create a Token
            SecurityTokenDescriptor descriptor = new() {

                // User for which the token is created
                Subject = new(claims),

                // Token life
                Expires = DateTime.Now.AddYears(1),

                // Digital signature for Encryption
                SigningCredentials=signature
            };

            // This is used to manage the token, creating, validating, renewing, revoking is done by the Token Handler
            JwtSecurityTokenHandler tokenHandler = new();

            // Creating Token using the Token handler
            SecurityToken token=tokenHandler.CreateToken(descriptor);

            // Outputting the token as a string because we return a string in this method
            return tokenHandler.WriteToken(token);
        }
    }
}