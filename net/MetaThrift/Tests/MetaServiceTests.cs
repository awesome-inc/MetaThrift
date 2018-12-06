using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace MetaThrift.Tests
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class MetaServiceTests
    {
        private const string ServiceName = "ServiceName";
        private const string ServiceDisplayName = "DisplayName";
        private const string ServiceDescription = "Description";
        static readonly DynamicMetaService Service = new DynamicMetaService(ServiceName, ServiceDisplayName, ServiceDescription);

        static MetaServiceTests()
        {
            Service.RegisterAction<string>("openBrowser", MockService.LaunchApp, "Open Browser", "Opens a web browser with the specified uri");
            Service.RegisterAction<string>("openVideo", MockService.LaunchApp, "Open Audio", "Opens a video player for the specified video url");
            Service.RegisterAction<string>("openAudio", MockService.LaunchApp, "Open Video", "Opens an audio player for the specified audio url");
            Service.RegisterAction("searchDoodles", () => MockService.LaunchApp("www.google.com/doodles/"), "Search Doodles", "Opens the Google Doodle page");

            Service.RegisterFunc<string, string>("sayHello", MockService.SayHello, "Greet Me", "Replies hello to the specified user");
            Service.RegisterFunc("sayHello", () => MockService.SayHello("everyone"), "Greetings", "Replies hello to everyone");
            Service.RegisterFunc<int, int>("fibonacci", MockService.Fibonacci, "Fibonacci", "Computes the Fibonacci series for the specfieid number");
            Service.RegisterFunc<int, int>("factorial", MockService.Factorial, "Factorial", "Computes the facorial for the specified number");
            Service.RegisterFunc<Tuple<int, int>, int>("add", MockService.Add, "Add", "Adds two integers");
            Service.RegisterFunc<Tuple<int, int, double>, double>("lerp", MockService.Lerp, "Lerp", "Linear interpolates the first two arguments by the third argument");
        }

        [Test]
        public void Should_reflect_name_and_description()
        {
            Service.Name.Should().Be(ServiceName);
            Service.DisplayName.Should().Be(ServiceDisplayName);
            Service.Description.Should().Be(ServiceDescription);

            var service = (MetaService.Iface)Service;
            service.getName().Should().Be(ServiceName);
            service.getDisplayName().Should().Be(ServiceDisplayName);
            service.getDescription().Should().Be(ServiceDescription);
        }

        [Test]
        public void Should_list_operations()
        {
            Trace.WriteLine("Operations:");
            foreach (var operation in Service.Operations)
                Trace.WriteLine(operation.PrettyPrint());

            var service = (MetaService.Iface)Service;
            service.getOperations().Should().BeEquivalentTo(Service.Operations);
        }

        [Test]
        public void Should_support_ping()
        {
            var sut = (MetaService.Iface) Service;
            sut.ping();
        }

        [Test]
        public void Should_call_operations()
        {
            if (!Service.Operations.Contains("openBrowser".ToMetaAction<string>()))
                Assert.Fail("The server does not support action \"openBrowser\".");

            const string url = "http://www.heise.de";
            Service.Call("openBrowser", url);

            if (!Service.Operations.Contains("fibonacci".ToMetaFunction<int, int>()))
                Assert.Fail("The server does not support function \"fibonacci\".");

            var userName = Environment.UserName;
            var strResult = Service.Call<string, string>("sayHello", userName);
            Trace.WriteLine(String.Format("sayHello({0}) = {1}", userName, strResult));

            var n = 20;
            var intResult = Service.Call<int, int>("fibonacci", n); // test xml
            Trace.WriteLine(String.Format("fibonacci({0}) = {1}", n, intResult));

            n = 6;
            intResult = Service.Call<int, int>("factorial", n);
            Trace.WriteLine(String.Format("factorial({0}) = {1}", n, intResult));

            int a = 3, b = 4;
            intResult = Service.Call<Tuple<int, int>, int>("add", new Tuple<int, int>(a, b) );
            Trace.WriteLine(String.Format("add({0},{1}) = {2}", a, b, intResult));

            a = -1; b = 1;
            const double t = 0.5;
            var doubleResult = Service.Call<Tuple<int, int, double>, double>("lerp", new Tuple<int, int, double>(a, b, t));
            Trace.WriteLine(String.Format("lerp({0},{1}, {2}) = {3}", a, b, t, doubleResult));
        }

        [Test]
        public void Should_call_void_action()
        {
            Service.Call("searchDoodles");
            Assert.True(true);
        }

        [Test]
        public void Should_call_void_function()
        {
            var msg = Service.Call<string>("sayHello");
            Trace.WriteLine(String.Format("sayHello(void) == {0}", msg));
            Assert.NotNull(msg);
        }

        [Test]
        public void Should_unregister_action()
        {
            var count = Service.Operations.Count();
            Service.UnregisterAction<string>("openBrowser");
            Service.Operations.Should().HaveCount(count - 1, "because we unregistered \"openBrowser(string)\"");

            Service.RegisterAction<string>("openBrowser", MockService.LaunchApp, "Open Browser", "Opens a web browser");
            Service.Operations.Should().HaveCount(count, "because we re-registered \"openBrowser(string)\"");
        }

        [Test]
        public void Should_unregister_function()
        {
            var count = Service.Operations.Count();
            Service.UnregisterFunc<string, string>("sayHello");
            Service.Operations.Should().HaveCount(count - 1, "because we unregistered \"sayHello(string)\"");

            Service.RegisterFunc<string, string>("sayHello", MockService.SayHello, "Greet Me", "Replies hello");
            Service.Operations.Should().HaveCount(count, "because we re-registered \"sayHello(string)\"");
        }

        [Test]
        public void Should_unregister_void_action()
        {
            var count = Service.Operations.Count();
            Service.UnregisterAction("searchDoodles");
            Service.Operations.Should().HaveCount(count - 1, "because we unregistered \"searchDoodles()\"");

            Service.RegisterAction("searchDoodles", () => MockService.LaunchApp("www.google.com/doodles/"), "Search Doodles", "Opens the Google Doodle page");
            Service.Operations.Should().HaveCount(count, "because we re-registered \"searchDoodles(string)\"");
        }

        [Test]
        public void Should_unregister_void_function()
        {
            var count = Service.Operations.Count();
            Service.UnregisterFunc<string>("sayHello");
            Service.Operations.Should().HaveCount(count - 1, "because we unregistered \"sayHello()\"");

            Service.RegisterFunc("sayHello", () => MockService.SayHello("everyone"), "Hello Everyone", "Says hello");
            Service.Operations.Should().HaveCount(count, "because we re-registered \"sayHello()\"");
        }

        [Test]
        public void Calling_unknown_signature_should_throw()
        {
            Service.Invoking(x => x.Call("dummy")).
                Should().Throw<ArgumentException>("unknown void action").
                And.Reason.Should().StartWith("The specified operation is not served by this instance:");

            Service.Invoking(x => x.Call<int>("sayHello")).
                Should().Throw<ArgumentException>("action with unknown signature called").
                And.Reason.Should().StartWith("The specified operation is not served by this instance:");

            Service.Invoking(x => x.Call<string, int>("sayHello", "input")).
                Should().Throw<ArgumentException>("function with unknown signature called").
                And.Reason.Should().StartWith("The specified operation is not served by this instance:");
        }

        [Test]
        public void Calling_with_input_of_mismatching_type_should_throw()
        {
            var sut = Service as MetaService.Iface;

            sut.Invoking(x => x.call("openBrowser".ToMetaAction<string>(), 1.ToMetaObject())).
                Should().Throw<ArgumentException>("method with invalid input (expected string instead of it)").
                And.Reason.Should().StartWith("The operation input type mismatches the type of the specified input");

            var sayHello = "sayHello".ToMetaFunction<string, string>();
            sut.Invoking(x => x.call(sayHello, 1.ToMetaObject())).
                Should().Throw<ArgumentException>("function with invalid input (expected string instead of it)").
                And.Reason.Should().StartWith("The operation input type mismatches the type of the specified input");

            var fibonacci = "fibonacci".ToMetaFunction<int, int>();
            var input = 1.ToMetaObject();
            input.Data = null;
            sut.Invoking(x => x.call(fibonacci, input)).
                Should().Throw<ArgumentException>("function with invalid input (should be a valid int but was null which is not allowed for int)").
                And.Reason.Should().StartWith("The specified input value could not be deserialized as the specified type");
        }

        [Test]
        public void Operation_exceptions_should_be_rethrown_to_client()
        {
            Service.RegisterAction("throw", () => { throw new InvalidOperationException("raised by test"); });
            try
            {
                Service.Invoking(x => x.Call("throw")).
                    Should().Throw<ServiceException>().
                    And.Reason.Should().StartWith("The operation failed:");
            }
            finally 
            {
                Service.UnregisterAction("throw");
            }
        }

        [Test]
        public void Operation_with_invalid_output_should_throw()
        {
            var service = Substitute.For<AbstractMetaService>();
            var intFunc = "intFunc".ToMetaFunction<int>();
            service.Operations.Returns(new[] {intFunc});
            service.Call(Arg.Any<MetaOperation>(), Arg.Any<object>()).Returns("output");

            var iface = (MetaService.Iface) service;
            iface.Invoking(x => x.call(intFunc, MetaObject.Empty)).
                Should().Throw<ArgumentException>().
                And.Reason.Should().StartWith("The specified output value could not be serialized to the specified type:");
        }
    }
}
