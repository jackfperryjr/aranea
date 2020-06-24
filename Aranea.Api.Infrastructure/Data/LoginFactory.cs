using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Models;

namespace Aranea.Api.Infrastructure.Data
{
    public class LoginFactory : IFactory<bool, LoginModel>
    {
        private UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private ApplicationDbContext _context;

        public LoginFactory(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<bool> GetAsync(LoginModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            var identity = await _userManager.FindByNameAsync(model.Username);
            if(identity != null && await _userManager.CheckPasswordAsync(identity, model.Password))
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
}