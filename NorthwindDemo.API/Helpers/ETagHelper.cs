using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NorthwindDemo.API.Helpers
{
    public static class ETagHelper
    {
        public static string Generate(object obj, int? page = null, int? pageSize = null)
        {
            var toHash = new
            {
                Data = obj,
                Page = page,
                PageSize = pageSize
            };

            var json = JsonSerializer.Serialize(toHash);

            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hash);
        }
    }
}
