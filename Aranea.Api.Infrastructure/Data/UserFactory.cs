using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Models;

namespace Aranea.Api.Infrastructure.Data
{
    public class UserFactory : IFactory<UserModel, string>
    {
        private UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private ApplicationDbContext _context;

        public UserFactory(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<UserModel> GetAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            UserModel user = new UserModel();
            var identity = _context.Users
                                    .Include(x => x.Photos)
                                    .Where(x => x.UserName == username)
                                    .FirstOrDefault();

            user.Id = identity.Id;
            user.UserName = identity.UserName;
            user.Email = identity.Email;
            user.Name = identity.Name;
            user.BirthDate = identity.BirthDate;
            user.Bio = identity.Bio;
            user.City = identity.City;
            user.State = identity.State;
            user.Country = identity.Country;
            user.RoleName = identity.RoleName;
            user.JoinDate = identity.JoinDate;
            user.Token = identity.Token;
            user.Photos = identity.Photos.OrderByDescending(x => x.Timestamp).ToList();

            return user;
        }
    }
}