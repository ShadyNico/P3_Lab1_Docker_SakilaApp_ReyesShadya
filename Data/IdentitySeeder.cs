using Microsoft.AspNetCore.Identity;

namespace SakilaApp.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // 1. Roles requeridos para el laboratorio
        string[] roles = 
        { 
            "Administrador", 
            "Supervisor", 
            "Operador", 
            "Consulta" 
        };

        // 2. Crear roles si todavía no existen
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 3. Usuarios de prueba con su contraseña y rol
        var usuarios = new[]
        {
            new
            {
                Email = "admin@espe.edu.ec",
                Password = "Admin123*",
                Role = "Administrador"
            },
            new
            {
                Email = "supervisor@espe.edu.ec",
                Password = "Supervisor123*",
                Role = "Supervisor"
            },
            new
            {
                Email = "operador@espe.edu.ec",
                Password = "Operador123*",
                Role = "Operador"
            },
            new
            {
                Email = "consulta@espe.edu.ec",
                Password = "Consulta123*",
                Role = "Consulta"
            }
        };

        // 4. Crear cada usuario y asignarle su rol correspondiente
        foreach (var item in usuarios)
        {
            var user = await userManager.FindByEmailAsync(item.Email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = item.Email,
                    Email = item.Email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, item.Password);
            }

            if (!await userManager.IsInRoleAsync(user, item.Role))
            {
                await userManager.AddToRoleAsync(user, item.Role);
            }
        }
    }
}