using System;
using System.Text;
using Micro.Auth.Api.GraphQL.Extensions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Micro.Auth.Api.GraphQL.Extensions
{
    public static class Tokens
    {
        public static string MustGetBearerToken(this IHttpContextAccessor httpContextAccessor)
        {
            var headerValue = httpContextAccessor.MustGetAuthHeader();
            if (!headerValue.StartsWith("Bearer "))
            {
                throw new InvalidTokenTypeException("Bearer");
            }
            return headerValue.Substring("Bearer ".Length).Trim();
        }

        private static string MustGetAuthHeader(this IHttpContextAccessor httpContextAccessor)
        {
            var header = StringValues.Empty;
            var exists = httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out header);
            if (exists == false || header.ToString() == "")
            {
                throw new MissingHeaderException("Authorization");
            }

            return header.ToString();
        }

        public static (string, string) MustGetBasicToken(this IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var headerValue = httpContextAccessor.MustGetAuthHeader();
                if (!headerValue.StartsWith("Basic "))
                {
                    throw new InvalidTokenTypeException("Basic");
                }

                var token = headerValue.Substring("Basic ".Length).Trim();
                var parts = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(":");
                return (parts[0], parts[1]);
            }
            catch (Exception e)
            {
                throw new InvalidTokenTypeException(e.Message);
            }
        }
    }
}
