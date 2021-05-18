using Library.API.Extensions;
using Library.API.Models;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Library.API.Testing
{
    /**
     * 集成测试(集成测试用于验证具有依赖关系的多个模块或组件是否能够正常工作)
     * 集成测试能够确保应用程序的组件正常工作，包括应用程序支持的基础结构，如数据库和文件系统等
     * 与单元测试不同，这里所有的依赖都是模拟出来的，在集成测试中，应使用与生产环境中一样的真实组件，如数据库和第三方库等
     */
    public class AuthorController_IntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        /// WebApplicationFactory类，用于创建内存中的测试服务器
        /// WebApplicationFactory类的CreateClient()方法能够创建HttpClient对象，在测试方法中，正是通过HttpClient对象所提供的方法对接口进行请求来完成测试
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly LoginUser _loginUser;

        public AuthorController_IntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _loginUser = new LoginUser
            {
                UserName = "demouser",
                Password = "demopwd",
                Email = "demouser@xxx.com"
            };
        }

        [Theory]
        [InlineData("48556951-e6b7-44fa-a24b-448cfc4c8c4c")]
        [InlineData("597e9f16-a810-4c34-98ff-92a53bbebcb9")]
        public async Task Test_GetAuthorById(string authorId)
        {
            // Arrange
            var client = _factory.CreateClient();
            var (result, token) = await client.TryGetBearerTokenAsync(_loginUser);
            if (!result)
            {
                throw new Exception("Authenication failed");
            }
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {token}");

            // Act
            var response = await client.GetAsync($"api/authors/{authorId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8; version=1.0", response.Content.Headers.ContentType.ToString());
            Assert.Contains(authorId, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Test_GetAuthorByNotExistId()
        {
            // Arrange
            var client = _factory.CreateClient();
            var (result, token) = await client.TryGetBearerTokenAsync(_loginUser);
            if (!result)
            {
                throw new Exception("Authenication failed");
            }
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {token}");

            // Act
            var response = await client.GetAsync($"api/authors/{Guid.NewGuid().ToString()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("12")]
        public async Task Test_GetAuthorByNotInvalidId(string authorId)
        {
            // Arrange
            var client = _factory.CreateClient();
            var (result, token) = await client.TryGetBearerTokenAsync(_loginUser);
            if (!result)
            {
                throw new Exception("Authenication failed");
            }
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {token}");

            // Act
            var response = await client.GetAsync($"api/authors/{authorId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Test_CreateAuthor_Unauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var authorDto = new AuthorDto
            {
                Name = "Test Author",
                Email = "author_testing@xx.com",
                BirthPlace = "Beijing",
                Age = 50
            };
            var jsonContent = JsonConvert.SerializeObject(authorDto);
            var (result, token) = await client.TryGetBearerTokenAsync(_loginUser);
            if (!result)
            {
                throw new Exception("Authenication failed");
            }
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {token}");

            // Act
            var response = await client.PostAsync($"api/authors", new StringContent(content: jsonContent, encoding: Encoding.UTF8, mediaType: "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
