using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.Internal.Configs
{
    public class IdentityConfig
    {
        public PasswordOptions PasswordRequirements { set; get; }
        public LockoutOptionsConfig LockoutOptions { set; get; }
        public UserOptions UserOptions { set; get; }
        public SignInOptions SignInOptions { set; get; }
        public string Issuer { set; get; }
        public string IssueForAudience { set; get; }
        public IEnumerable<string> Audiences { set; get; }
    }

    public class LockoutOptionsConfig
    {
        public bool AllowedForNewUsers { set; get; }
        public int DefaultLockoutTimeSpanInSeconds { set; get; }
        public int MaxFailedAccessAttempts { set; get; }
    }

    public static class IdentityConfigExtension
    {
        public static LockoutOptions ToIdentityLockoutOptions(this LockoutOptionsConfig options)
        {
            return new LockoutOptions
            {
                AllowedForNewUsers = options.AllowedForNewUsers,
                DefaultLockoutTimeSpan = TimeSpan.FromSeconds(options.DefaultLockoutTimeSpanInSeconds),
                MaxFailedAccessAttempts = options.MaxFailedAccessAttempts
            };
        }
    }
}
