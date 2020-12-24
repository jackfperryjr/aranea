using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountStore(
            UserManager<ApplicationUserModel> userManager,
            SignInManager<ApplicationUserModel> signInManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApplicationUserModel> AddAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUserModel> UpdateAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            var portrait = ApplicationExtensions.ConfigureBlobContainer( // Container for portraits.
                                    0,
                                    _configuration["StorageConfig:AccountName"], 
                                    _configuration["StorageConfig:AccountKey"]); 
            await portrait.CreateIfNotExistsAsync();
            var wallpaper = ApplicationExtensions.ConfigureBlobContainer( // Container for wallpapers.
                                    1,
                                    _configuration["StorageConfig:AccountName"], 
                                    _configuration["StorageConfig:AccountKey"]); 
            await wallpaper.CreateIfNotExistsAsync();

            IFormFileCollection files;
            try 
            {
                files = _httpContextAccessor.HttpContext.Request.Form.Files;
                if (files.Count != 0) 
                {
                    for (var i = 0; i < files.Count; i++)
                    {
                        if (files[i].Name == "portrait")
                        {
                            var newID = Guid.NewGuid();
                            var newBlob = portrait.GetBlockBlobReference(newID + ".png");
                            using (var filestream = new MemoryStream())
                            {   
                                files[i].CopyTo(filestream);
                                filestream.Position = 0;
                                await newBlob.UploadFromStreamAsync(filestream);
                            }

                            PhotoModel photo = new PhotoModel()
                            {
                                Id = newID,
                                Url = "https://rikku.blob.core.windows.net/portrait/" + newID + ".png",
                                Portrait = 1,
                                UserId = user.Id
                            };

                            await _context.AddAsync(photo);
                            await _context.SaveChangesAsync();
                        }

                        if (files[i].Name == "wallpaper")
                        {
                            var newID = Guid.NewGuid();
                            var newBlob = wallpaper.GetBlockBlobReference(newID + ".png");
                            using (var filestream = new MemoryStream())
                            {   
                                files[i].CopyTo(filestream);
                                filestream.Position = 0;
                                await newBlob.UploadFromStreamAsync(filestream);
                            }

                            PhotoModel photo = new PhotoModel()
                            {
                                Id = newID,
                                Url = "https://rikku.blob.core.windows.net/wallpaper/" + newID + ".png",
                                Wallpaper = 1,
                                UserId = user.Id
                            };
                                                    
                            await _context.AddAsync(photo);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            catch
            {
                // TODO:
            }

            // user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.City = model.City;
            user.State = model.State;
            user.BirthDate = Convert.ToDateTime(model.BirthDate);
            user.Age = model.Age;
            user.Profile = model.Profile;
            user.Email = model.Email;
            user.LoggedInIP = model.LoggedInIP;
            var result = await _userManager.UpdateAsync(user);
            return model;
        }

        public async Task<ApplicationUserModel> DeleteAsync(ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            var roles = await _userManager.GetRolesAsync(user);
            var photos = await _context.AspNetUserPhotos.Where(x => x.UserId == model.Id).ToListAsync();
 
            foreach (var role in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            _context.AspNetUserPhotos.RemoveRange(photos);
            await _userManager.DeleteAsync(user);
            _context.SaveChanges();
            return model;
        }
    }
}