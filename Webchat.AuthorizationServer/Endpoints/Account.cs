using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Webchat.AuthorizationServer.Data;
using Webchat.AuthorizationServer.Models;

namespace Webchat.AuthorizationServer.Endpoints
{
    public static class Account
    {
        public static RouteGroupBuilder MapAccountEndpoints(this RouteGroupBuilder builder)
        {
            builder.MapPost("register", Register);

            return builder;
        }

        private static async Task<IResult> Register(
            [FromForm] RegistrationModel model,
            UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
            {
                return Results.Conflict();
            }
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Results.Ok(result);
            }
            return Results.BadRequest(result);
        }
    }
}
