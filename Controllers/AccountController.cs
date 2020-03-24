using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Aranea.Data;
using Aranea.Models;

namespace Aranea.Controllers
{
    // This controller will perform login and logout only.
    [Route("api")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            UserModel user = new UserModel();
            var identity = await _userManager.FindByNameAsync(model.Username);
            if(user != null && await _userManager.CheckPasswordAsync(identity, model.Password))
            {
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, identity.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Token")));
                var token = new JwtSecurityToken(
                    issuer: "Aranea", // This is the name of this project API.
                    audience: model.Audience,
                    expires: DateTime.Now.AddHours(6),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                user.UserName = identity.UserName;
                user.FirstName = identity.FirstName;
                user.LastName = identity.LastName;
                user.BirthDate = identity.BirthDate;
                user.Age = identity.Age;
                user.City = identity.City;
                user.State = identity.State;
                user.Country = identity.Country;
                user.Photo = identity.Photo;
                user.Wallpaper = identity.Wallpaper;
                user.RoleName = identity.RoleName;
                user.JoinDate = identity.JoinDate;

                return Ok(new
                {
                    status = 200,
                    message = "User logged in successfully.",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = user
                });
            }
            return BadRequest(new
            {
                status = 404,
                message = "User is logged out or does not exist."
            });
        }

        [HttpPost] 
        [Route("logout")]
        public async Task<IActionResult> Logout() 
        { 
            await _signInManager.SignOutAsync(); 
            return Ok(new 
            {
                status = 200,
                message = "User logged out successfully."
            });        
        } 
    }
}