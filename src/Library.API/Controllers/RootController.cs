using Library.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    /**
     * HATEOAS >> Hypermedia As Engine Of Application State(超媒体作为应用程序状态引擎)
     */
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        public ActionResult<List<Link>> GetRoot()
        {
            var links = new List<Link>();

            links.Add(new Link(HttpMethods.Get.ToString(),
                "self",
                Url.Link(nameof(GetRoot), null)));

            links.Add(new Link(HttpMethods.Get.ToString(),
                "get authors",
                Url.Link(nameof(AuthorController.GetAuthorsAsync), null)));

            links.Add(new Link(HttpMethods.Post.ToString(),
                "create author",
                Url.Link(nameof(AuthorController.CreateAuthorAsync), null)));

            return Ok(links);
        }
    }
}
