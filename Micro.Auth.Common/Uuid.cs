namespace Micro.Auth.Common
{
    public interface IUuidService
    {
        string GenerateUuId();
    }

    public class UuidService : IUuidService
    {
        public string GenerateUuId()
        {
            return System.Guid.NewGuid().ToString();
        }
    }
}

