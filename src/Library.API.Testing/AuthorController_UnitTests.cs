using AutoMapper;
using Library.API.Controllers;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Library.API.Testing
{
    /**
     * 单元测试(单元测试是指验证代码段（如方法或函数）功能的测试，通常由开发人员编写相应的测试方法，以验证代码执行后与预期结果是否一致)
     * 单元测试由开发人员完成，主要用来测试程序中的类以及其中的方法是否能够正确运行。在添加单元测试方法时，应遵循Arrange-Act-Assert模式，使测试方法的代码更加规范
     * Arrange：为测试进行准备操作，如设置测试数据、变量和环境等。
     * Act：执行要测试的方法，如调用要测试的函数和方法
     * Assert：断言测试结果，验证被测试方法的输出是否与预期的结果一致。
     * 
     * 
     * 系统测试是对整个系统进行全面测试，以确认系统正常运行并符合需求
     */
    public class AuthorController_UnitTests
    {
        private AuthorController _authorController;
        private Mock<IDistributedCache> _mockDistributedCache;
        private Mock<ILogger<AuthorController>> _mockLogger;
        private Mock<IUrlHelper> _mockUrlHelper;
        private Mock<IMapper> _mockMapper;
        private Mock<IAuthorRepository> _mockAuthorRepository;

        public AuthorController_UnitTests()
        {
            _mockDistributedCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<AuthorController>>();
            _mockUrlHelper = new Mock<IUrlHelper>();
            _mockMapper = new Mock<IMapper>();
            _mockAuthorRepository = new Mock<IAuthorRepository>();
            _authorController = new AuthorController(_mockMapper.Object, _mockDistributedCache.Object, _mockAuthorRepository.Object);
            _authorController.ControllerContext = new ControllerContext
            {
                // 已实例化的AuthorController的Response属性默认为空
                // Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));会抛异常
                // 因此为AuthorController对象的ControllerContext属性设置了一个ControllerContext对象
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task Tets_GetAllAuthors()
        {
            // Arrange
            var author = new Author
            {
                Id = Guid.NewGuid(),
                Name = "Author Test 1",
                Email = "author1@xxx.com",
                BirthPlace = "Beijing"
            };

            var authorDto = new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Email = author.Email,
                BirthPlace = author.BirthPlace
            };

            var authorList = new List<Author> { author };
            var authorDtoList = new List<AuthorDto> { authorDto };
            var parameters = new AuthorResourceParameters();
            var authors = new PagedList<Author>(authorList, totalCount: authorList.Count, pageNumber: parameters.PageNumber, pageSize: parameters.PageSize);
            _mockAuthorRepository.Setup(m => m.GetAllAsync(It.IsAny<AuthorResourceParameters>())).Returns(Task.FromResult(authors));
            _mockMapper.Setup(m => m.Map<IEnumerable<AuthorDto>>(It.IsAny<IEnumerable<Author>>())).Returns(authorDtoList);
            _mockUrlHelper.Setup(m => m.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("demo url");
            _authorController.Url = _mockUrlHelper.Object;

            // Act
            var actionResult = await _authorController.GetAuthorsAsync(parameters);

            // Assert
            ResourceCollect<AuthorDto> resourceCollect = actionResult.Value;
            Assert.True(1 == resourceCollect.Items.Count);
            Assert.Equal(authorDto, resourceCollect.Items[0]);
            Assert.True(_authorController.Response.Headers.ContainsKey("X-Pagination"));
        }
    }
}
