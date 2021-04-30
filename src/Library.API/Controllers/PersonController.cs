using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    /// <summary>
    /// Action级别版本控制
    /// </summary>
    [Route("api/persons")]
    [ApiController]
    [ApiVersion("1.0")]
    //[ApiVersion("1.0", Deprecated = true)]  // Deprecated属性为true时，该接口标识不再使用；api-deprecated-versions中包含该版本信息，但是接口仍可访问，只是在问来一段时间内，可能不再提供
    [ApiVersion("2.0")]
    public class PersonController : ControllerBase
    {
        //[HttpGet]
        //public ActionResult<string> Get() => "result from v1";

        [HttpGet, MapToApiVersion("2.0")]
        public ActionResult<string> GetV2() => "result from v2";

        [HttpGet("version")]
        public ActionResult<ApiVersion> RequestedApiVersion() => HttpContext.GetRequestedApiVersion();
    }
}
