using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Aranea.Data
{
    public static class ApplicationRole
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
    }
}