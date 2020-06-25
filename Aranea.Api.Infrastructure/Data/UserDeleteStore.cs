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
    public class UserDeleteStore : IStore<ApplicationUserModel>
    {
        private UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private ApplicationDbContext _context;

        public UserDeleteStore(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<ApplicationUserModel> AddAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUserModel> UpdateAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUserModel> DeleteAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            var roles = await _userManager.GetRolesAsync(user);
 
            foreach (var role in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            var result = await _userManager.DeleteAsync(user);

            return model;
        }
    }
}