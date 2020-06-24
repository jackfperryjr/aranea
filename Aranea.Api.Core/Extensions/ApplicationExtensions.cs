using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

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
    }
}
