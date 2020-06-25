using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Aranea.Api.Core.Extensions;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Models;

namespace Aranea.Api.Infrastructure.Data
{
    public class AccountStore : IStore<ApplicationUserModel>
    {
        private UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private ApplicationDbContext _context;
        private IConfiguration _configuration;

        public AccountStore(
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

        public async Task<ApplicationUserModel> AddAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUserModel> UpdateAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            // // This container is used for blob storage uploads.
            // var container = ApplicationExtensions.ConfigureBlobContainer(
            //                         _configuration["StorageConfig:AccountName"], 
            //                         _configuration["StorageConfig:AccountKey"]); 
            // await container.CreateIfNotExistsAsync();

            // if (model.UserName != null && model.UserName != user.UserName)
            // {
            //     user.UserName = model.UserName;
            // }

            if (model.FirstName != null && model.FirstName != user.FirstName)
            {
                user.FirstName = model.FirstName;
            }

            if (model.LastName != null && model.LastName != user.LastName)
            {
                user.LastName = model.LastName;
            }

            if (model.City != null && model.City != user.City)
            {
                user.City = model.City;
            }

            if (model.State != null && model.State != user.State)
            {
                user.State = model.State;
            }

            if (model.BirthDate != null && Convert.ToDateTime(model.BirthDate) != user.BirthDate)
            {
                user.BirthDate = Convert.ToDateTime(model.BirthDate);
            }

            if (model.Age != 0 && model.Age != user.Age)
            {
                user.Age = model.Age;
            }

            if (model.Profile != null && model.Profile != user.Profile)
            {
                user.Profile = model.Profile;
            }

            if (model.Email != null && model.Email != user.Email)
            {
                user.Email = model.Email;
            }
                
            var result = await _userManager.UpdateAsync(user);

            return model;
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