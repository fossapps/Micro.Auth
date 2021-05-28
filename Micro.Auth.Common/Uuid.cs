namespace Micro.Auth.Common
{
    public interface IUuidService
    {
        string GenerateUuId(string prefix);
    }

    public class UuidService : IUuidService
    {
        private static string GenerateUuId()
        {
            return System.Guid.NewGuid().ToString();
        }

        public string GenerateUuId(string prefix)
        {
            return $"{prefix}_{GenerateUuId()}";
        }
    }
}

