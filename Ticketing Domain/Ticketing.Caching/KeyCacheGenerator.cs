using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Caching
{
    public class KeyCacheGenerator
    {
        public static string GenerateKey(string requestUrl, string query)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(requestUrl);

            if (!string.IsNullOrEmpty(query))
            {
                keyBuilder.Append('|' + query[1..]);
            }

            return keyBuilder.ToString();
        }

        public static string GenerateKey(HttpRequest request)
        {
            return GenerateKey(request.Path, request.QueryString.Value);
        }
    }
}
