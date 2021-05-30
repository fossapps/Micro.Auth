using Micro.Auth.Business.Internal.Keys;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Keys
{
    public class KeyContainerTest
    {
        [Test]
        public void TestCanStoreAndRetrieveKeyFromKeyContainer()
        {
            var container = new KeyContainer();
            container.SetKey(new SigningKey
            {
                KeyId = "first"
            });
            var key = container.GetKey();
            Assert.AreEqual("first", key.KeyId);
        }

        [Test]
        public void TestStoringKeyReplacesOldOne()
        {
            var container = new KeyContainer();
            container.SetKey(new SigningKey
            {
                KeyId = "first"
            });
            container.SetKey(new SigningKey
            {
                KeyId = "second"
            });
            var key = container.GetKey();
            Assert.AreEqual("second", key.KeyId);
        }
    }
}
