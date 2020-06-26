using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Aranea.Api.Core.Abstractions;
using Aranea.Api.Core.Models;

namespace Aranea.Api.Core.Extensions
{
    public class ApplicationExtensions
    {
        public static CloudBlobContainer ConfigureBlobContainer(string account, string key)
        {
            // Configures container based on credentials passed in.
            var storageCredentials = new StorageCredentials(account, key);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference("images");
            return container;
        }

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

        public async static Task GetLoginLocationData(string username, IHttpContextAccessor _httpContextAccessor, UserManager<ApplicationUserModel> _userManager, IStore<ApplicationUserModel> _accountStore)
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
    }
}
