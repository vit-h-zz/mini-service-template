using System;
using System.Collections.Generic;
using System.Linq;
using Common.Application.Abstractions;
using IdentityModel;

namespace MiniService.Tests.Helpers
{
    public class TestUserContext : IUserContext
    {
        public TestUserContext(string testUserId)
        {
            Claims[JwtClaimTypes.Subject] = new List<string> { testUserId };
        }

        public Dictionary<string, List<string>> Claims { get; } = new();
        public string? GetUserIdOrDefault() => GetClaims(JwtClaimTypes.Subject).FirstOrDefault();
        public string GetUserId() => GetUserIdOrDefault() ?? throw new InvalidOperationException("No user id has been set.");
        public IEnumerable<string> GetClaims(string claimType) => Claims.GetValueOrDefault(claimType) ?? Enumerable.Empty<string>();
    }
}
