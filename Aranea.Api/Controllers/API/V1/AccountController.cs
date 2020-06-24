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
    public class AccountController : ApiControllerBase
    {
        private readonly IFactory<bool, LoginModel> _loginFactory;
        private readonly IFactory<string, string> _logoutFactory;
        private readonly IFactory<UserModel, string> _userFactory;
        private readonly IStore<RegisterModel> _userStore;
        private readonly IFactory<JwtSecurityToken, LoginModel> _tokenFactory;

        public AccountController(
            IFactory<bool, LoginModel> loginFactory,
            IFactory<string, string> logoutFactory,
            IFactory<UserModel, string> userFactory,
            IStore<RegisterModel> userStore,
            IFactory<JwtSecurityToken, LoginModel> tokenFactory)
        {
            _loginFactory = loginFactory;
            _logoutFactory = logoutFactory;
            _userFactory = userFactory;
            _userStore = userStore;
            _tokenFactory = tokenFactory;
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
            if (isLogin) 
            {
                var user = await _userFactory.GetAsync(model.Username, cancellationToken);
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