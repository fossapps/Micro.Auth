using System;

namespace Micro.Auth.Business.Users
{
    public class User
    {
        public string Id { set; get; }
        public string Email { set; get; }
        public string UserName { set; get; }
        public bool EmailConfirmed { set; get; }
        public DateTime? LockoutEnd { set; get; }
    }
}
