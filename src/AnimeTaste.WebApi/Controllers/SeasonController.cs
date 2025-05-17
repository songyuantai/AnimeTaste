using AnimeTaste.Core;
using AnimeTaste.Core.Model;
using AnimeTaste.WebApi.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeTaste.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonController(MinioService service) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("upload")]
        public async Task<Result<bool>> UploadTestFile()
        {
            var result = new Result<bool>();
            await service.UploadFile("anime", "D:\\test.jpg");
            //await service.CheckBucket1("9090");
            return result.Ok("上传成功！");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dir")]
        public async Task<string?> GetTestFileUrl()
        {
            return await service.GetFileUrl("anime", "test.jpg");
        }
    }
}
