using Micro.Auth.Common;
using NUnit.Framework;

namespace Micro.Auth.UnitTest.Uuid
{
    public class UuidServiceTest
    {
        [Test]
        public void TestCreatesRandomString()
        {
            var service = new UuidService();
            Assert.IsInstanceOf<string>(service.GenerateUuId("prefix"));
            Assert.AreNotEqual(service.GenerateUuId("prefix"), service.GenerateUuId("prefix"));
        }

        [Test]
        public void TestCreatesRandomStringWithPrefix()
        {
            var service = new UuidService();
            Assert.IsInstanceOf<string>(service.GenerateUuId("prefix"));
            Assert.True(service.GenerateUuId("prefix").StartsWith("prefix"));
        }
    }
}
