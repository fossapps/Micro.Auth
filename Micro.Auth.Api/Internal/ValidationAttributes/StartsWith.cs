using System;
using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Api.Internal.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class StartsWith : ValidationAttribute
    {
        private readonly string _prefix;

        public StartsWith(string prefix) : base($"The {{0}} field must start with {prefix}")
        {
            _prefix = prefix;
        }

        public override bool IsValid(object value)
        {
            return value != null && value is string stringVal && stringVal.StartsWith(_prefix);
        }
    }
}
