using AnimeTaste.Core.Model;
using AnimeTaste.WebApi;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AnimeTaste.Auth
{
    public class ExternalAuthStateProvider(
        IJSRuntime js,
        ApiClient client) : AuthenticationStateProvider
    {

        private ClaimsPrincipal currentUser = new(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(currentUser));

        public async Task<Result<LoginInOut>?> LogInAsync(string userNo, string password)
        {
            var loginOut = await client.LoginAsync(userNo, password);
            var loginTask = LogInAsyncCore(loginOut);
            NotifyAuthenticationStateChanged(loginTask);

            return loginOut;
        }

        private async Task<AuthenticationState> LogInAsyncCore(Result<LoginInOut>? loginOut)
        {
            if (null != loginOut && null != loginOut.Data && loginOut.IsSuccess)
            {
                var token = loginOut!.Data!.Token ?? "";
                await js.InvokeAsync<string>("localStorage.setItem", ApiClient.AUTH_KEY, token);
                var claims = ParseToken(token);
                var identity = new ClaimsIdentity(claims);
                currentUser = new ClaimsPrincipal(identity);
                return await Task.FromResult(new AuthenticationState(currentUser));
            }
            else
            {
                currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                return await Task.FromResult(new AuthenticationState(currentUser));
            }
        }

        public async Task Logout()
        {
            await js.InvokeAsync<string>("localStorage.removeItem", ApiClient.AUTH_KEY);
            currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(currentUser)));
        }

        public static IEnumerable<Claim> ParseToken(string jwtToken)
        {
            try
            {
                // 定义用于验证的密钥，这里的密钥需要与签发JWT时使用的密钥一致
                var key = "sPQDHra3UMMedCVfxKwcjhHWARSznRAv";
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "WebAppIssuer", // 颁发者
                    ValidAudience = "WebAppAudience", // 接收者
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)) // 密钥
                };

                // 解析令牌
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(jwtToken, tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    return jwtSecurityToken.Claims;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
            }

            return [];
        }
    }
}
