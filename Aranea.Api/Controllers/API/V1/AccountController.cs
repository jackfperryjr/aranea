using System;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Aranea.Api.Core.Models;
using Aranea.Api.Core.WebApi;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Extensions;

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
        private readonly IFactory<object[], LoginModel> _tokenFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<ApplicationUserModel> _userManager;
        private IConfiguration _configuration;

        public AccountController(
            IFactory<bool, LoginModel> loginFactory,
            IFactory<string, string> logoutFactory,
            IFactory<UserModel, string> userFactory,
            IStore<RegisterModel> userStore,
            IStore<ApplicationUserModel> accountStore,
            IFactory<object[], LoginModel> tokenFactory,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUserModel> userManager,
            IConfiguration configuration)
        {
            _loginFactory = loginFactory;
            _logoutFactory = logoutFactory;
            _userFactory = userFactory;
            _userStore = userStore;
            _accountStore = accountStore;
            _tokenFactory = tokenFactory;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            RegisterModel newUser = new RegisterModel();

            try 
            {
                newUser = await _userStore.AddAsync(model, cancellationToken);
            }
            catch
            {
                return BadRequest(new
                {
                    message = "An error occurred processing the registration."
                });
            }

            if(newUser.Username != null) 
            {
                LoginModel login = new LoginModel
                {
                    Username = model.Username,
                    Password = model.Password,
                    Audience = model.Audience
                };

                var isLogin = await _loginFactory.GetAsync(login, cancellationToken);

                try 
                {
                    if (isLogin) 
                    {
                        await ApplicationExtensions.GetLoginLocationData(model.Username, _httpContextAccessor, _userManager, _accountStore);
                        var user = await _userFactory.GetAsync(login.Username, cancellationToken);
                        var tokens = await _tokenFactory.GetAsync(login, cancellationToken);

                        return Ok(new
                        {
                            message = "User logged in successfully.",
                            accessToken = tokens[0],
                            refreshToken = tokens[1],
                            expiration = tokens[2],
                            user = user
                        });
                    }
                    else 
                    {
                        return Unauthorized(new
                        {
                            message = "User login failed."
                        });
                    }
                }
                catch 
                {
                    return BadRequest();
                }
            }
            else 
            {
                return BadRequest(new
                {
                    message = "Something weird happened."
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken = new CancellationToken())
        {
            var isLogin = await _loginFactory.GetAsync(model, cancellationToken);

            try 
            {
                if (isLogin) 
                {
                    await ApplicationExtensions.GetLoginLocationData(model.Username, _httpContextAccessor, _userManager, _accountStore);
                    var tokens = await _tokenFactory.GetAsync(model, cancellationToken);
                    var user = await _userFactory.GetAsync(model.Username, cancellationToken);

                    return Ok(new
                    {
                        message = "User logged in successfully.",
                        accessToken = tokens[0],
                        refreshToken = tokens[1],
                        expiration = tokens[2],
                        user = user
                    });
                }
                else 
                {
                    return Unauthorized(new
                    {
                        message = "User login failed."
                    });
                }
            }
            catch 
            {
                return BadRequest();
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

        [HttpPost("refresh")] 
        public async Task<IActionResult> Refresh([FromBody] string refreshToken, CancellationToken cancellationToken = new CancellationToken()) 
        { 
            var data = ApplicationExtensions.GetTokenData(_httpContextAccessor);
            var user = await _userFactory.GetAsync(data[0].ToString(), cancellationToken);

            if (refreshToken == user.Token)
            {
                LoginModel model = new LoginModel() 
                {
                    Username = data[0].ToString(),
                    Password = String.Empty,
                    Audience = data[1].ToString()
                };

                var tokens = await _tokenFactory.GetAsync(model, cancellationToken);

                return Ok(new 
                {
                        message = "Tokens refreshed successfully.",
                        accessToken = tokens[0],
                        refreshToken = tokens[1],
                        expiration = tokens[2]
                });
            } 
            else 
            {
                return BadRequest(new
                {
                    message = "Invalid refresh token."
                });
            }   
        } 
    }
}