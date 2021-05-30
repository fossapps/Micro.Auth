using System;

namespace Micro.Auth.Business.Users
{
    public class User
    {
        public static User FromDbUser(Micro.Auth.Storage.User user)
        {
            return new User
            {
                Email = user.Email,
                Id = user.Id,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnd = user.LockoutEnd?.DateTime,
                UserName = user.UserName
            };
        }
        public string Id { set; get; }
        public string Email { set; get; }
        public string UserName { set; get; }
        public bool EmailConfirmed { set; get; }
        public DateTime? LockoutEnd { set; get; }
    }
}
