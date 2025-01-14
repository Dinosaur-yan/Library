﻿using Library.API.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = "RegisteredMoreThan3Days")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        public ActionResult<List<Link>> GetRoot()
        {
            var links = new List<Link>
            {
                new Link(HttpMethods.Get.ToString(),
                "self",
                Url.Link(nameof(GetRoot), null)),

                new Link(HttpMethods.Get.ToString(),
                "get authors",
                Url.Link(nameof(AuthorController.GetAuthorsAsync), null)),

                new Link(HttpMethods.Post.ToString(),
                "create author",
                Url.Link(nameof(AuthorController.CreateAuthorAsync), null))
            };

            return Ok(links);
        }
    }
}
