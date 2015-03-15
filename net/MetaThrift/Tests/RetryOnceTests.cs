using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace MetaThrift.Tests
{
    [TestFixture]
    class RetryOnceTests
    {
        [Test]
        public void Call_should_retry_on_handled_exceptions()
        {
            var service = Substitute.For<MetaService.Iface>();
            var operation = "dummy".ToMetaAction();
            var input = MetaObject.Empty;
            var output = MetaObject.Empty;

            service.call(operation, input).Returns(
                x => { throw new InvalidOperationException("raised by test"); },
                x => output);
            
            int factoryCalls = 0;
            var sut = new RetryOnce<InvalidOperationException>(() =>
            {
                factoryCalls++;
                return service;
            });

            sut.call(operation, input);
            service.Received(2).call(operation, input);
            factoryCalls.Should().Be(2);
        }

        [Test]
        public void Call_should_rethrow_unhandled_exceptions()
        {
            var service = Substitute.For<MetaService.Iface>();
            var operation = "dummy".ToMetaAction();
            var input = MetaObject.Empty;

            service.call(operation, input).
                Returns(x => { throw new System.ArgumentException("raised by test"); });

            var sut = new RetryOnce<InvalidOperationException>(() => service);

            sut.Invoking(x => x.call(operation, input))
                .ShouldThrow<System.ArgumentException>()
                .WithMessage("raised by test");
        }
    }
}