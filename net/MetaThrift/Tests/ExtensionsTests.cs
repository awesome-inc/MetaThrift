using FluentAssertions;
using NUnit.Framework;

namespace MetaThrift.Tests
{
    [TestFixture]
    class ExtensionsTests
    {
        [Test]
        public void Wrap_operation_should_prepend_service_name()
        {
            const string serviceName = "SomeService";
            var op = "hello".ToMetaAction<string>().Wrap(serviceName);
            op.Name.Should().Be(serviceName + "/hello");
            op.GetServiceName().Should().Be(serviceName);
            op.Unwrap().Name.Should().Be("hello");
        }
    }
}