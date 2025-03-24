using AnimeTaste.Core;
using AnimeTaste.Core.Model;
using AnimeTaste.Service;
using AnimeTaste.WebApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTaste.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserService service) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<Result<LoginInOut>> Login(LoginIn loginIn)
        {
            var result = App.Resolve<Result<LoginInOut>>();
            var user = await service.GetUser(loginIn.UserNo, loginIn.Password);
            if (user == null)
            {
                return result!.Fail("用户名或者密码错误!");
            }

            var claimList = await service.GetClaimsAsync(user);
            var token = App.Resolve<JwtHelper>()!.CreateToken(claimList);

            //cache.setToken

            var data = new LoginInOut
            {
                Token = token
            };
            return result!.Ok("登录成功！", data);

        }

    }
}
