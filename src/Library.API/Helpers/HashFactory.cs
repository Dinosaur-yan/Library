using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Library.API.Helpers
{
    public class HashFactory
    {
        public static string GetHash(object obj)
        {
            string result = string.Empty;
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            var bytes = Encoding.UTF8.GetBytes(json);

            using (var hasher = MD5.Create())
            {
                var hash = hasher.ComputeHash(bytes);
                result = BitConverter.ToString(hash);
                result = result.Replace("-", "");
            }

            return result;
        }
    }
}
