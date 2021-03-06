using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Models;

namespace Aranea.Api.Core.Extensions
{
    public class ApplicationExtensions
    {
        public static void CreateRoles(IServiceProvider serviceProvider, IConfiguration Configuration)
        {
            string[] roles = { "Admin", "SuperUser", "User", "Banned" };

            foreach (string role in roles)
            {
                CreateRole(serviceProvider, role);
            }
        }

        private static void CreateRole(IServiceProvider serviceProvider, string role)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            Task<bool> exists = roleManager.RoleExistsAsync(role);
            exists.Wait();

            if (!exists.Result)
            {
                Task<IdentityResult> result = roleManager.CreateAsync(new IdentityRole(role));
                result.Wait();
            }
        }

        public static CloudBlobContainer ConfigureBlobContainer(int blob, string account, string key)
        {
            // Configures container based on credentials passed in.
            // blob: 0 for portrait, 1 for wallpaper.
            var storageCredentials = new StorageCredentials(account, key);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            if (blob == 1) 
            {
                var container = cloudBlobClient.GetContainerReference("wallpaper");
                return container;
            }
            else 
            {
                var container = cloudBlobClient.GetContainerReference("portrait");
                return container;
            }
        }

        public async static Task GetLoginLocationData(
            string username, 
            IHttpContextAccessor _httpContextAccessor, 
            UserManager<ApplicationUserModel> _userManager, 
            IStore<ApplicationUserModel> _accountStore)
        {
            var userIp = string.Empty;

            if (_httpContextAccessor.HttpContext.Request.Headers != null)
            {
                var forwardedHeader = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];

                if (!StringValues.IsNullOrEmpty(forwardedHeader))
                {
                    userIp = forwardedHeader.FirstOrDefault();
                }
            }

            if (string.IsNullOrEmpty(userIp) && _httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
            {
                userIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            CancellationToken cancellationToken = new CancellationToken();
            var appUser = await _userManager.FindByNameAsync(username);
            appUser.LoggedInIP = userIp;
            
            // TODO:
            // appUser.LoggedInCity = userCity;
            // appUser.LoggedInRegion = userRegion;
            // appUser.LoggedInCountry = userCountry;

            await _accountStore.UpdateAsync(appUser, cancellationToken);
        }

        public static string GetAccessTokenFromHeaders(IHttpContextAccessor _httpContextAccessor)
        {
            return _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }

        public static object[] GetTokenData(IHttpContextAccessor _httpContextAccessor)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(
                                GetAccessTokenFromHeaders(_httpContextAccessor)
                            );

            var sub = token.Claims.First(x => x.Type == "sub").Value;
            var aud = token.Claims.First(x => x.Type == "aud").Value;
            object[] data = {sub, aud};
            return data;
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var x = RandomNumberGenerator.Create())
            {
                x.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
