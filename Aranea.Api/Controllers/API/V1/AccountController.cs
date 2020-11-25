using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
                    var token = await _tokenFactory.GetAsync(model, cancellationToken);
                    var user = await _userFactory.GetAsync(model.Username, cancellationToken);
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return Ok(new
                    {
                        message = "User logged in successfully.",
                        token = tokenString,
                        expiration = token.ValidTo,
                        refreshToken = user.Token,
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

        [Obsolete]
        [HttpGet("api/{value}")] 
        public async Task<IActionResult> GetApiValue(string value, CancellationToken cancellationToken = new CancellationToken()) 
        { 
            try
            {
                if (value == "simplewebrtc")
                {
                    return Ok("https://api.simplewebrtc.com/config/guest/152fc7a1fc48368c4c948f9e");
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
    }
}