using Micro.Auth.Api.Uuid;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Uuid
{
    public class UuidServiceTest
    {
        [Test]
        public void TestCreatesRandomString()
        {
            var service = new UuidService();
            Assert.IsInstanceOf<string>(service.GenerateUuId());
            Assert.AreNotEqual(service.GenerateUuId(), service.GenerateUuId());
        }
    }
}
