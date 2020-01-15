using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Aranea.Data;
using Aranea.Models;

namespace Aranea.Controllers
{
    // This controller will perform actions that involve anything except login or logout.
    [Route("api")]
    [Authorize]
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _configuration;

        public ManageController(
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
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            ApplicationUser user = new ApplicationUser 
            { 
                UserName = model.Username, 
                Email = model.Email, 
                FirstName = model.FirstName,
                Picture = "https://rikku.blob.core.windows.net/images/default-avatar.png",
                Wallpaper = "https://rikku.blob.core.windows.net/images/default-wallpaper.png",
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
                    message = "User registered successfully."
                });
            }
            else 
            {
                return BadRequest(new 
                {
                    status = 400,
                    message = "Unable to register user."
                });
            }     
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            IList<string> roles;

            if (user == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Unable to find user."
                });
            }
            else 
            {
                roles = await _userManager.GetRolesAsync(user);
            }

            if (roles.Contains("Admin") || roles.Contains("User")) // If user is in Admin or User role.
            {
                if (roles.Count() > 0)
                {
                    foreach (var role in roles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded) 
                {
                    return Ok(new
                    {
                        status = 200,
                        message = "User account removed successfully."
                    });
                }
                else 
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "Could not complete request. (Most likely user doesn't exist.)"
                    });
                }
            }
            else 
            {
                return Unauthorized(new
                {
                    status = 401,
                    message = "User is not authorized."
                });
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] ApplicationUser model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            IList<string> roles;

            // This container is used for blob storage uploads.
            var container = ApplicationHelper.ConfigureBlobContainer(
                                    _configuration["StorageConfig:AccountName"], // Saved in appsettings.json
                                    _configuration["StorageConfig:AccountKey"]); // Saved in appsettings.json
            await container.CreateIfNotExistsAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = "Unable to find user."
                });
            }
            else 
            {
                roles = await _userManager.GetRolesAsync(user);
            }

            if (roles.Contains("Admin") || roles.Contains("User")) // If user is in Admin or User role.
            {
                if (model.UserName != null && model.UserName != user.UserName)
                {
                    user.UserName = model.UserName;
                }

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

                if (model.Age != null && model.Age != user.Age)
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
                if (result.Succeeded) 
                {
                    return Ok(new
                    {
                        status = 200,
                        message = "User updated successfully."
                    });
                }
                else 
                {
                    return BadRequest(new
                    {
                        status = 400,
                        message = "User could not be updated."
                    });
                }
            }
            else 
            {
                return Unauthorized(new
                {
                    status = 401,
                    message = "User is not authorized."
                });
            }
        }
    }
}