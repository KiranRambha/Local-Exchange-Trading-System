using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace LETS.Helpers
{
    public class UserAuthentication
    {
        public ClaimsIdentity AuthenticateUser(string username, string role = "user")
        {
            var ident = new ClaimsIdentity( new[] { 
                // adding following 2 claim just for supporting default antiforgery provider
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
              },
              DefaultAuthenticationTypes.ApplicationCookie);

            return ident;
        }
    }
}