using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Models;
using Aranea.Api.Core.Extensions;

namespace Aranea.Api.Infrastructure.Data
{
    public class TokenFactory : IFactory<object[], LoginModel>
    {
        private UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private ApplicationDbContext _context;
        private IConfiguration _configuration;


        public TokenFactory(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<object[]> GetAsync(LoginModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            var identity = await _userManager.FindByNameAsync(model.Username);
            var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, identity.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            var roles = await _userManager.GetRolesAsync(identity);
            authClaims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value));

            var token = new JwtSecurityToken(
                issuer: "chocoboAPI", 
                audience: model.Audience,
                expires: DateTime.Now.AddMinutes(30),
                claims: authClaims,
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var refreshToken = ApplicationExtensions.GenerateRefreshToken();
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            identity.Token = refreshToken;//new JwtSecurityTokenHandler().WriteToken(token);
            await _context.SaveChangesAsync();

            object[] tokens = {tokenString, refreshToken, token.ValidTo};

            return tokens;
        }
    }
}