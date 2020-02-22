namespace Micro.Auth.Api.Keys
{
    public interface IKeyContainer
    {
        void SetKey(SigningKey key);
        SigningKey? GetKey();
    }

    public class KeyContainer : IKeyContainer
    {
        private SigningKey _key;

        public void SetKey(SigningKey key)
        {
            _key = key;
        }

        public SigningKey GetKey()
        {
            return _key;
        }
    }
}
