using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AnimeTaste.Auth
{
    public class ExternalAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(currentUser));

        public Task LogInAsync()
        {
            var loginTask = LogInAsyncCore();
            NotifyAuthenticationStateChanged(loginTask);

            return loginTask;

            async Task<AuthenticationState> LogInAsyncCore()
            {
                var user = await LoginWithExternalProviderAsync();
   
                currentUser = user;

                return new AuthenticationState(currentUser);
            }
        }

        private Task<ClaimsPrincipal> LoginWithExternalProviderAsync()
        {
            /*
                Provide OpenID/MSAL code to authenticate the user. See your identity 
                provider's documentation for details.

                Return a new ClaimsPrincipal based on a new ClaimsIdentity.
            */
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());

            return Task.FromResult(authenticatedUser);
        }

        public void Logout()
        {
            currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(currentUser)));
        }
    }
}
