using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Identity;
using Aranea.Api.Core.Models;
using Aranea.Api.Core.WebApi;
using Aranea.Api.Core.Abstractions;

namespace Aranea.Api.Controllers.API.V1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    public class AccountController : ApiControllerBase
    {
        private readonly IFactory<bool, LoginModel> _loginFactory;
        private readonly IFactory<string, string> _logoutFactory;
        private readonly IFactory<UserModel, string> _userFactory;
        private readonly IStore<RegisterModel> _userStore;
        private readonly IStore<ApplicationUserModel> _accountStore;
        private readonly IFactory<JwtSecurityToken, LoginModel> _tokenFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<ApplicationUserModel> _userManager;

        public AccountController(
            IFactory<bool, LoginModel> loginFactory,
            IFactory<string, string> logoutFactory,
            IFactory<UserModel, string> userFactory,
            IStore<RegisterModel> userStore,
            IStore<ApplicationUserModel> accountStore,
            IFactory<JwtSecurityToken, LoginModel> tokenFactory,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUserModel> userManager)
        {
            _loginFactory = loginFactory;
            _logoutFactory = logoutFactory;
            _userFactory = userFactory;
            _userStore = userStore;
            _accountStore = accountStore;
            _tokenFactory = tokenFactory;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            try 
            {
                var newUser = await _userStore.AddAsync(model, cancellationToken);

                if(newUser.Username != null) 
                {
                    LoginModel login = new LoginModel
                    {
                        Username = model.Username,
                        Password = model.Password,
                        Audience = model.Audience
                    };

                    var isLogin = await _loginFactory.GetAsync(login, cancellationToken);

                    if (isLogin) 
                    {
                        var user = await _userFactory.GetAsync(login.Username, cancellationToken);
                        var token = await _tokenFactory.GetAsync(login, cancellationToken);
                        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                        return Ok(new
                        {
                            message = "User logged in successfully.",
                            token = tokenString,
                            expiration = token.ValidTo,
                            user = user
                        });
                    }
                    else 
                    {
                        return BadRequest(new
                        {
                            message = "User is logged out or does not exist."
                        });
                    }
                }
                else 
                {
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            var isLogin = await _loginFactory.GetAsync(model, cancellationToken);
            var userIp = string.Empty;

            if (_httpContextAccessor.HttpContext.Request.Headers != null)
            {
                var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
                if (!StringValues.IsNullOrEmpty(forwardedHeader))
                    userIp = forwardedHeader.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(userIp) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
            {
                userIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            if (isLogin) 
            {
                var user = await _userFactory.GetAsync(model.Username, cancellationToken);
                var appUser = await _userManager.FindByNameAsync(model.Username);
                
                appUser.LoggedInIP = userIp;
                await _accountStore.UpdateAsync(appUser, cancellationToken);

                var token = await _tokenFactory.GetAsync(model, cancellationToken);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    message = "User logged in successfully.",
                    token = tokenString,
                    expiration = token.ValidTo,
                    user = user
                });
            }
            else 
            {
                return BadRequest(new
                {
                    message = "User is logged out or does not exist."
                });
            }
        }

        [HttpPost("logout")] 
        public async Task<IActionResult> Logout(string model, CancellationToken cancellationToken = new CancellationToken()) 
        { 
            model = "";
            await _logoutFactory.GetAsync(model, cancellationToken);
            return Ok(new 
            {
                message = "User logged out successfully."
            });        
        } 
    }
}