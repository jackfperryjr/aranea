using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aranea.Api.Core.Models;
using Aranea.Api.Core.WebApi;
using Aranea.Api.Core.Abstractions;

namespace Aranea.Api.Controllers.API.V1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    public class ManageController : ApiControllerBase
    {
        private readonly IFactory<bool, LoginModel> _loginFactory;
        private readonly IFactory<UserModel, string> _userFactory;
        private readonly IStore<RegisterModel> _userStore;

        public ManageController(
            IFactory<bool, LoginModel> loginFactory,
            IFactory<UserModel, string> userFactory,
            IStore<RegisterModel> userStore)
        {
            _loginFactory = loginFactory;
            _userFactory = userFactory;
            _userStore = userStore;
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] ApplicationUserModel model)
        {
            // var user = await _userManager.FindByIdAsync(model.Id);
            // IList<string> roles;

            // if (user == null)
            // {
            //     return NotFound(new
            //     {
            //         status = 404,
            //         message = "Unable to find user."
            //     });
            // }
            // else 
            // {
            //     roles = await _userManager.GetRolesAsync(user);
            // }

            // if (roles.Contains("Admin") || roles.Contains("User")) // If user is in Admin or User role.
            // {
            //     if (roles.Count() > 0)
            //     {
            //         foreach (var role in roles)
            //         {
            //             await _userManager.RemoveFromRoleAsync(user, role);
            //         }
            //     }

            //     var result = await _userManager.DeleteAsync(user);

            //     if (result.Succeeded) 
            //     {
            //         return Ok(new
            //         {
            //             status = 200,
            //             message = "User account removed successfully."
            //         });
            //     }
            //     else 
            //     {
            //         return BadRequest(new
            //         {
            //             status = 400,
            //             message = "Could not complete request. (Most likely user doesn't exist.)"
            //         });
            //     }
            // }
            // else 
            // {
            //     return Unauthorized(new
            //     {
            //         status = 401,
            //         message = "User is not authorized."
            //     });
            // }

            return BadRequest();
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] ApplicationUserModel model)
        {
            // var user = await _userManager.FindByIdAsync(model.Id);
            // IList<string> roles;

            // // This container is used for blob storage uploads.
            // var container = ApplicationExtensions.ConfigureBlobContainer(
            //                         _configuration["StorageConfig:AccountName"], // Saved in appsettings.json
            //                         _configuration["StorageConfig:AccountKey"]); // Saved in appsettings.json
            // await container.CreateIfNotExistsAsync();

            // if (user == null)
            // {
            //     return NotFound(new
            //     {
            //         status = 404,
            //         message = "Unable to find user."
            //     });
            // }
            // else 
            // {
            //     roles = await _userManager.GetRolesAsync(user);
            // }

            // if (roles.Contains("Admin") || roles.Contains("User")) // If user is in Admin or User role.
            // {
            //     if (model.UserName != null && model.UserName != user.UserName)
            //     {
            //         user.UserName = model.UserName;
            //     }

            //     if (model.FirstName != null && model.FirstName != user.FirstName)
            //     {
            //         user.FirstName = model.FirstName;
            //     }

            //     if (model.LastName != null && model.LastName != user.LastName)
            //     {
            //         user.LastName = model.LastName;
            //     }

            //     if (model.City != null && model.City != user.City)
            //     {
            //         user.City = model.City;
            //     }

            //     if (model.State != null && model.State != user.State)
            //     {
            //         user.State = model.State;
            //     }

            //     if (model.BirthDate != null && Convert.ToDateTime(model.BirthDate) != user.BirthDate)
            //     {
            //         user.BirthDate = Convert.ToDateTime(model.BirthDate);
            //     }

            //     if (model.Age != 0 && model.Age != user.Age)
            //     {
            //         user.Age = model.Age;
            //     }

            //     if (model.Profile != null && model.Profile != user.Profile)
            //     {
            //         user.Profile = model.Profile;
            //     }

            //     if (model.Email != null && model.Email != user.Email)
            //     {
            //         user.Email = model.Email;
            //     }
                
            //     var result = await _userManager.UpdateAsync(user);
            //     if (result.Succeeded) 
            //     {
            //         return Ok(new
            //         {
            //             status = 200,
            //             message = "User updated successfully."
            //         });
            //     }
            //     else 
            //     {
            //         return BadRequest(new
            //         {
            //             status = 400,
            //             message = "User could not be updated."
            //         });
            //     }
            // }
            // else 
            // {
            //     return Unauthorized(new
            //     {
            //         status = 401,
            //         message = "User is not authorized."
            //     });
            // }

            return BadRequest();
        }
    }
}