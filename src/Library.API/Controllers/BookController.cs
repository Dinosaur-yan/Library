using Library.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Library.API.Controllers
{
    /// <summary>
    /// book
    /// </summary>
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        /// <summary>
        /// 获取所有book资源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string[]> GetBooks([FromRoute] Guid authorId)
        {
            return new string[] { "value1", "values2" };
        }

        /// <summary>
        /// 获取book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpGet("{bookId}", Name = nameof(GetBook))]
        public ActionResult<string> GetBook([FromRoute] Guid authorId, Guid bookId)
        {
            return "value";
        }

        /// <summary>
        /// 创建book资源
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateBook([FromRoute] Guid authorId, [FromBody] BookForCreationDto bookForCreationDto)
        {
            return CreatedAtRoute(nameof(GetBook), new { });
        }

        /// <summary>
        /// 删除book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpDelete("{bookId}")]
        public ActionResult<string[]> DeleteBook([FromRoute] Guid authorId, Guid bookId)
        {
            return NoContent();
        }

        /// <summary>
        /// 全量更新book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        [HttpPut("{bookId}")]
        public ActionResult<string[]> UpdateBook([FromRoute] Guid authorId, Guid bookId)
        {
            return NoContent();
        }

        /// <summary>
        /// 局部更新book资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="bookId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{bookId}")]
        public ActionResult<string[]> PartiallyUpdateBook([FromRoute] Guid authorId, Guid bookId, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            return NoContent();
        }
    }
}
