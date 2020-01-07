using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Aranea.Data;
using Aranea.Models;

namespace Aranea.Controllers
{
    [Route("api")]
    public class AuthenticateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _configuration;

        public AuthenticateController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if(user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {

                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Token")));
                var audience = model.Issuer;

                var token = new JwtSecurityToken(
                    issuer: model.Issuer,
                    audience: audience,
                    expires: DateTime.Now.AddHours(6),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    status = 200,
                    message = "User logged in.",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized(new
            {
                status = 401,
                message = "User is logged out or does not exist."
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {

            ApplicationUser user = new ApplicationUser 
                    { 
                        UserName = model.Username, 
                        Email = model.Email, 
                        FirstName = model.FirstName,
                        JoinDate = DateTime.Now
                    };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var users = _userManager.Users.ToList();

                if (users.Count == 1) {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    user.RoleName = "Admin";
                    await _userManager.UpdateAsync(user);
                }
                else 
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    user.RoleName = "User";
                    await _userManager.UpdateAsync(user);
                }
                await _context.SaveChangesAsync();

                return Ok(new 
                {
                    status = 200,
                    message = "User registered."
                });
            }
            else 
            {
                return BadRequest(new 
                {
                    status = 400,
                    message = "User not registered."
                });
            }     
        }

        [HttpPost] 
        [Route("logout")]
        public async Task<IActionResult> Logout() 
        { 
            await _signInManager.SignOutAsync(); 
            return Ok(new 
            {
                status = 200,
                message = "User logged out."
            });        
        } 
    }
}