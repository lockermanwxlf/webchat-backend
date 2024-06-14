using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Webchat.AuthorizationServer.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Webchat.AuthorizationServer.Endpoints
{
    public static class Authorization
    {
        public static RouteGroupBuilder MapAuthorizationEndpoints(this RouteGroupBuilder builder)
        {
            builder.MapPost("token", Exchange);
            return builder;
        }

        private static async Task<IResult> Exchange(
            HttpContext httpContext,
            IOpenIddictApplicationManager applicationManager,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            var request = httpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The openid request could not be retrieved.");
            var application = await applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("The client id is invalid.");
            if (request.IsPasswordGrantType())
            {
                var user = await userManager.FindByEmailAsync(request.Username);
                var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
                if (!result.Succeeded)
                {
                    return Results.BadRequest(result);
                }
                var identity = new ClaimsIdentity(authenticationType: TokenValidationParameters.DefaultAuthenticationType, nameType: Claims.Name, roleType: Claims.Role);
                identity.SetClaim(Claims.Subject, user.Id);
                identity.SetClaim(Claims.Email, user.Email);
                identity.SetClaim(Claims.ClientId, request.ClientId);
                identity.SetScopes(request.GetScopes());
                identity.SetDestinations(claim => [Destinations.AccessToken]);
                return Results.SignIn(principal: new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            }
            throw new NotImplementedException("The requested grant type is not implemented.");
        }
    }
}
