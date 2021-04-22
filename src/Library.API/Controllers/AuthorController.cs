using Library.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Library.API.Controllers
{
    /// <summary>
    /// author
    /// </summary>
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        /// <summary>
        /// 获取所有author资源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string[]> GetAuthors()
        {
            return new string[] { "value1", "values2" };
        }

        /// <summary>
        /// 获取author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpGet("{authorId}", Name = nameof(GetAuthor))]
        public ActionResult<string> GetAuthor([FromRoute] Guid authorId)
        {
            return "value";
        }

        /// <summary>
        /// 创建author资源
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateAuthor([FromBody] AuthorForCreationDto authorForCreationDto)
        {
            return CreatedAtRoute(nameof(GetAuthor), new { });
        }

        /// <summary>
        /// 删除author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpDelete("{authorId}")]
        public ActionResult<string[]> DeleteAuthor([FromRoute] Guid authorId)
        {
            return NoContent();
        }
    }
}
