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

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            await _accountStore.DeleteAsync(model, cancellationToken);
            return Ok(new 
            {
                message = "User removed successfully."
            });  
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ApplicationUserModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            await _accountStore.UpdateAsync(model, cancellationToken);
            return Ok(new 
            {
                message = "User updated successfully."
            });  
        }
    }
}