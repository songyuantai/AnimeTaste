using AnimeTaste.Core;
using AnimeTaste.Core.Model;
using AnimeTaste.Model;
using AnimeTaste.ViewModel.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace AnimeTaste.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController(ISqlSugarClient db) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("SystemRoleOptions")]
        public async Task<Result<List<SelectOption>>> GetSystemRoleSelectOptions()
        {
            var list = (await db.Queryable<SystemRole>().ToListAsync())
                .Select(m => new SelectOption
                {
                    Text = m.RoleName,
                    Value = m.Id.ToString(),
                }).ToList();
            return new Result<List<SelectOption>>().Ok("获取成功！", list);
        }
    }
}
