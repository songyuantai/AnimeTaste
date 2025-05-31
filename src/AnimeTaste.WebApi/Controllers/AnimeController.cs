using AnimeTaste.Core;
using AnimeTaste.Core.Model;
using AnimeTaste.Service;
using AnimeTaste.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTaste.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeController(AnimeService animeService) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("collect")]
        public async Task<Result<bool>> AnimeCollectToggle(AnimeCollectOption option)
        {
            await animeService.AnimeCollectToggle(option.AnimeId, option.SeasonId, option.DayOfWeek, option.IsCollect);
            return new Result<bool>().Ok();
        }



    }
}
