using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Library.API.Controllers
{
    [Route("api/values")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private List<Student> students = new List<Student>();

        public ValuesController(IDataProtectionProvider dataProtectionProvider, ILogger<ValuesController> logger)
        {
            DataProtectionProvider = dataProtectionProvider;
            Logger = logger;
            students.Add(new Student { Id = "1", Name = "TestUser" });
        }

        public IDataProtectionProvider DataProtectionProvider { get; }
        public ILogger Logger { get; }

        [HttpGet]
        public ActionResult<IEnumerable<Student>> Get()
        {
            //var protector = DataProtectionProvider.CreateProtector("ProtectResourceId");
            var protectorA = DataProtectionProvider.CreateProtector("A");
            var protectorB = DataProtectionProvider.CreateProtector("B");
            var protector = DataProtectionProvider.CreateProtector("C");

            var result = students.Select(s => new Student
            {
                Id = protector.Protect(s.Id),
                Name = s.Name
            });
            return result.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Student> Get(string id)
        {
            //var protector = DataProtectionProvider.CreateProtector("ProtectResourceId");
            var protector = DataProtectionProvider.CreateProtector("A", "B", "C");
            var rawId = protector.Unprotect(id);
            var targetItem = students.FirstOrDefault(s => s.Id == rawId);
            return new Student { Id = id, Name = targetItem.Name };
        }

        private void TimeLimitedDataProtectorTest()
        {
            //当使用Unprotect方法解密时，如果密文已经过期，则同样会抛出CryptographicException异常
            var protector = DataProtectionProvider.CreateProtector("testing").ToTimeLimitedDataProtector();
            var content = protector.Protect("Hello", DateTimeOffset.Now.AddMinutes(10));
            try
            {
                var rawContent = protector.Unprotect(content, out DateTimeOffset expiration);
            }
            catch (CryptographicException ex)
            {
                Logger.LogError(ex.Message, ex);
            }

            /*
             Microsoft.AspNetCore.DataProtection包中还提供了EphemeralDataProtectionProvider类，作为IDataProtectionProvider接口的一个实现，
             它的加密和解密功能具有“一次性”的特点，当密文不需要持久化时，可以使用这种方式。所有的键都存储在内存中，且每个EphemeralDataProtectionProvider实例都有自己的主键。
             */
        }
    }

    public class Student
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
