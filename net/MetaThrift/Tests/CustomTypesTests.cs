using System;
using FluentAssertions;
using NUnit.Framework;

namespace MetaThrift.Tests
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    internal class CustomTypesTests
    {
        [Test]
        public void Assert_RegisterType()
        {
            const string someTypeName = "de.awesomeinc.someType";
            Register.Type<SomeType>(someTypeName);

            0.Invoking(x => Register.Type<SomeType>(someTypeName)).Should().Throw<System.ArgumentException>();
            1.Invoking(x => Register.Type<int>(someTypeName)).Should().Throw<System.ArgumentException>();

            var expected = new SomeType { Uri = "http://www.heise.de" };
            var mo = expected.ToMetaObject();
            mo.TypeName.Should().BeEquivalentTo(someTypeName);

            var actual = mo.FromMetaObject<SomeType>();
            actual.Should().BeEquivalentTo(expected);

            Unregister.Type<SomeType>();

            mo = expected.ToMetaObject();
            mo.TypeName.Should().BeEquivalentTo(typeof(SomeType).FullName);

            2.Invoking(x => Unregister.Type<SomeType>()).Should().Throw<System.ArgumentException>();
        }

        private class SomeType : IEquatable<SomeType>
        {
            // ReSharper disable MemberCanBePrivate.Local
            public string Uri { get; set; }
            // ReSharper restore MemberCanBePrivate.Local

            public bool Equals(SomeType other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Uri, other.Uri);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((SomeType)obj);
            }

            public override int GetHashCode()
            {
                return (Uri != null ? Uri.GetHashCode() : 0);
            }
        }
    }
}
