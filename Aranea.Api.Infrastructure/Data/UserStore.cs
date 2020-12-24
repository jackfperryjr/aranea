using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Models;

namespace Aranea.Api.Infrastructure.Data
{
    public class UserStore : IStore<RegisterModel>
    {
        private UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private ApplicationDbContext _context;

        public UserStore(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<RegisterModel> AddAsync(RegisterModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            ApplicationUserModel user = new ApplicationUserModel
            { 
                UserName = model.Username, 
                Email = model.Email, 
                FirstName = model.FirstName,
                JoinDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);
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

            return model;
        }

        public async Task<RegisterModel> UpdateAsync(RegisterModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterModel> DeleteAsync(RegisterModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}