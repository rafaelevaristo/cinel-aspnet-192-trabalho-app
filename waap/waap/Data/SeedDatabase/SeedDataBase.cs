using Microsoft.AspNetCore.Identity;
using wapp;

namespace waap.Data.SeedDataBase;
 public class SeedDatabase
    {
        public static void Seed(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager).Wait();
            SeedUsers(userManager).Wait();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
    // CREATE ADMIN
    var roleCheck = await roleManager.RoleExistsAsync(waapConstants.ROLES.ADMIN);
    if (!roleCheck)
    {
        var adminRole = new IdentityRole
        {
            Name = waapConstants.ROLES.ADMIN
        };
        await roleManager.CreateAsync(adminRole);
    }

    // CREATE SLAESMAN
    roleCheck = await roleManager.RoleExistsAsync(waapConstants.ROLES.SALESMAN);
    if (!roleCheck)
    {
        var driverRole = new IdentityRole
        {
            Name = waapConstants.ROLES.SALESMAN
        };
        await roleManager.CreateAsync(driverRole);
    }

    // CREATE LOGISTICS
    roleCheck = await roleManager.RoleExistsAsync(waapConstants.ROLES.LOGISTICS);
    if (!roleCheck)
    {
        var administrativeRole = new IdentityRole
        {
            Name = waapConstants.ROLES.LOGISTICS
        };
        await roleManager.CreateAsync(administrativeRole);
    }
}

private static async Task SeedUsers(UserManager<IdentityUser> userManager)
{
    // Seed Admin User
    var dbAdmin = await userManager.FindByNameAsync(waapConstants.USERS.ADMIN.USERNAME);

    if (dbAdmin == null)
    {
        IdentityUser userAdmin = new IdentityUser
        {
            UserName = waapConstants.USERS.ADMIN.USERNAME,
            Email = waapConstants.USERS.ADMIN.EMAIL
        };

        var result = await userManager.CreateAsync(userAdmin, waapConstants.USERS.ADMIN.PASSWORD);

        if (result.Succeeded)
        {
            dbAdmin = await userManager.FindByNameAsync(waapConstants.USERS.ADMIN.USERNAME);
            await userManager.AddToRoleAsync(dbAdmin, waapConstants.ROLES.ADMIN);
        }
    }

    // Seed SALESMAN User
    var dbSalesMan = await userManager.FindByNameAsync(waapConstants.USERS.SALESMAN.USERNAME);

    if (dbSalesMan == null)
    {
        IdentityUser userSalesman = new IdentityUser
        {
            UserName = waapConstants.USERS.SALESMAN.USERNAME,
            Email = waapConstants.USERS.SALESMAN.EMAIL
        };

        var result = await userManager.CreateAsync(userSalesman, waapConstants.USERS.SALESMAN.PASSWORD);

        if (result.Succeeded)
        {
            dbSalesMan = await userManager.FindByNameAsync(waapConstants.USERS.SALESMAN.USERNAME);
            await userManager.AddToRoleAsync(dbSalesMan, waapConstants.ROLES.SALESMAN);
        }
    }

    // Seed Administrative User
    var dbLogistics = await userManager.FindByNameAsync(waapConstants.USERS.LOGISTICS.USERNAME);

    if (dbLogistics == null)
    {
        IdentityUser userLogistics = new IdentityUser
        {
            UserName = waapConstants.USERS.LOGISTICS.USERNAME,
            Email = waapConstants.USERS.LOGISTICS.EMAIL
        };

        var result = await userManager.CreateAsync(userLogistics, waapConstants.USERS.LOGISTICS.PASSWORD);

        if (result.Succeeded)
        {
            dbLogistics = await userManager.FindByNameAsync(waapConstants.USERS.LOGISTICS.USERNAME);
            await userManager.AddToRoleAsync(dbLogistics, waapConstants.ROLES.LOGISTICS);
        }
    }
}

    }