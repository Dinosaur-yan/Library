using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    /// <summary>
    /// author
    /// </summary>
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public AuthorController(IMapper mapper, IAuthorRepository authorRepository)
        {
            Mapper = mapper;
            AuthorRepository = authorRepository;
        }

        public IMapper Mapper { get; }
        public IAuthorRepository AuthorRepository { get; }

        /// <summary>
        /// 获取所有author资源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<AuthorDto>>> GetAuthors()
        {
            var authors = (await AuthorRepository.GetAllAsync()).OrderBy(author => author.Name);

            var authorDtoList = Mapper.Map<IEnumerable<AuthorDto>>(authors);
            return authorDtoList.ToList();
        }

        /// <summary>
        /// 获取author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpGet("{authorId}", Name = nameof(GetAuthor))]
        public async Task<ActionResult<AuthorDto>> GetAuthor([FromRoute] Guid authorId)
        {
            var author = await AuthorRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }
            else
            {
                return Mapper.Map<AuthorDto>(author);
            }
        }

        /// <summary>
        /// 创建author资源
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateAuthor([FromBody] AuthorForCreationDto authorForCreationDto)
        {
            var author = Mapper.Map<Author>(authorForCreationDto);

            AuthorRepository.Create(author);
            var result = await AuthorRepository.SaveAsync();
            if (!result)
            {
                throw new Exception("创建资源author失败");
            }

            var authorDto = Mapper.Map<AuthorDto>(author);
            return CreatedAtRoute(nameof(GetAuthor), new { authorId = authorDto.Id }, authorDto);
        }

        /// <summary>
        /// 删除author资源
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpDelete("{authorId}")]
        public async Task<ActionResult> DeleteAuthor([FromRoute] Guid authorId)
        {
            var author = await AuthorRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }

            AuthorRepository.Delete(author);
            var result = await AuthorRepository.SaveAsync();
            if (!result)
            {
                throw new Exception("删除资源author失败");
            }

            return NoContent();
        }
    }
}
