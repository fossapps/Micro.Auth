using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.PasswordManager
{
    public class PasswordResetFailedException : Exception
    {
        public PasswordResetFailedException(IEnumerable<IdentityError> resultErrors)
        :base(String.Join(':', resultErrors.Select(e => e.Description)))
        {
        }
    }
}
