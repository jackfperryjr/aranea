using System;
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
    [Authorize]
    public class ManageController : ApiControllerBase
    {
        private readonly IFactory<bool, LoginModel> _loginFactory;
        private readonly IFactory<UserModel, string> _userFactory;
        private readonly IStore<RegisterModel> _userStore;
        private readonly IStore<ApplicationUserModel> _accountStore;

        public ManageController(
            IFactory<bool, LoginModel> loginFactory,
            IFactory<UserModel, string> userFactory,
            IStore<RegisterModel> userStore,
            IStore<ApplicationUserModel> accountStore)
        {
            _loginFactory = loginFactory;
            _userFactory = userFactory;
            _userStore = userStore;
            _accountStore = accountStore;
        }

        [AllowAnonymous]
        [Obsolete]
        [HttpGet("get/{userName}")]
        public async Task<IActionResult> Get(string userName, CancellationToken cancellationToken = new CancellationToken())
        {
            try 
            {
                var user = await _userFactory.GetAsync(userName, cancellationToken);
                return Ok(new
                {
                    message = "User details retrieved successfully.",
                    user = user
                });
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await _accountStore.DeleteAsync(model, cancellationToken);
                return Ok(new 
                {
                    message = "User removed successfully."
                });  
            }
            catch 
            {
                return BadRequest();
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            if (model.Id == null || model.UserName == null)
            {
                return BadRequest(new
                {
                    message = "A username and id is required."
                });
            }

            try
            {
                await _accountStore.UpdateAsync(model, cancellationToken);
                var user = await _userFactory.GetAsync(model.UserName, cancellationToken);
                return Ok(new 
                {
                    message = "User updated successfully.",
                    user = user
                });  
            }
            catch
            {
                return BadRequest(new
                {
                    message = "An error occurred processing the update."
                });
            }
        }
    }
}