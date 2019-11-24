using System;
using System.IO;
using System.Security.Cryptography;
using PemUtils;

namespace Micro.Auth.Api.Keys
{
    public class SigningKey
    {
        public RSA PrivateKey { set; get; }
        public string PublicKey { set; get; }
        public string KeyId { set; get; }
        public DateTime CreatedAt { set; get; }

        public static SigningKey Create()
        {
            var rsa = RSA.Create(2048);
            return new SigningKey
            {
                CreatedAt = DateTime.Now,
                PrivateKey = rsa,
                PublicKey = GetPublicKey(rsa.ExportParameters(false))
            };
        }

        private static string GetPublicKey(RSAParameters publicKey)
        {
            var stream = new MemoryStream();
            var writer = new PemWriter(stream);
            writer.WritePublicKey(publicKey);
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
