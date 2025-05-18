using AnimeTaste.Service;
using AnimeTaste.ViewModel.Ui;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTaste.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonController(SeasonService seasonService) : ControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        [Route("season_list_option")]
        public async Task<List<LabeledValue>> GetSeasonOptionList()
        {

            var list = await seasonService.GetOrAddSeasonList();
            var data = list.Select(m => new LabeledValue(m.Id.ToString(), m.Name ?? "", false)).ToList();
            return data;
        }
    }
}
