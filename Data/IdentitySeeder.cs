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

        // 4. Crear o reparar cada usuario y asignarle su rol correspondiente.
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

                EnsureSucceeded(
                    await userManager.CreateAsync(user, item.Password),
                    $"crear el usuario {item.Email}");
            }
            else
            {
                var changed = false;

                if (user.UserName != item.Email)
                {
                    EnsureSucceeded(
                        await userManager.SetUserNameAsync(user, item.Email),
                        $"actualizar el nombre de usuario de {item.Email}");
                    changed = true;
                }

                if (user.Email != item.Email)
                {
                    EnsureSucceeded(
                        await userManager.SetEmailAsync(user, item.Email),
                        $"actualizar el correo de {item.Email}");
                    changed = true;
                }

                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    changed = true;
                }

                if (user.TwoFactorEnabled)
                {
                    user.TwoFactorEnabled = false;
                    changed = true;
                }

                if (changed)
                {
                    EnsureSucceeded(
                        await userManager.UpdateAsync(user),
                        $"actualizar el usuario {item.Email}");
                }

                var hasPassword = await userManager.HasPasswordAsync(user);
                if (!hasPassword)
                {
                    EnsureSucceeded(
                        await userManager.AddPasswordAsync(user, item.Password),
                        $"asignar contrasena a {item.Email}");
                }
                else if (!await userManager.CheckPasswordAsync(user, item.Password))
                {
                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    EnsureSucceeded(
                        await userManager.ResetPasswordAsync(user, resetToken, item.Password),
                        $"restablecer contrasena de {item.Email}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, item.Role))
            {
                EnsureSucceeded(
                    await userManager.AddToRoleAsync(user, item.Role),
                    $"asignar el rol {item.Role} a {item.Email}");
            }
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string action)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"No se pudo {action}: {errors}");
    }
}
