using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace MetaThrift.Tests
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class SerializationTests
    {

        [Test]
        public void ToFromType_should_support_void()
        {
            TestType(typeof(void), String.Empty);
        }

        [Test]
        public void ToFromType_should_support_primitive_types()
        {
            TestType<sbyte>("byte");
            TestType<bool>("boolean");
            TestType<DateTime>("date");
            TestType<string>("string");
            TestType<decimal>("decimal");
            TestType<double>("double");
            TestType<float>("float");
            TestType<int>("int");
            TestType<long>("long");
            TestType<short>("short");
        }

        [Test]
        public void ToFromType_should_support_unsigned_integral_types()
        {
            // NOTE: unsigned types not supported in Java!
            TestType<byte>("ubyte");
            TestType<uint>("uint");
            TestType<ulong>("ulong");
            TestType<ushort>("ushort");
        }

        [Test]
        public void ToFromType_should_support_arrays_of_primitive_types()
        {
            TestType<sbyte[]>("array<byte>");
            TestType<bool[]>("array<boolean>");
            TestType<DateTime[]>("array<date>");
            TestType<string[]>("array<string>");
            TestType<decimal[]>("array<decimal>");
            TestType<double[]>("array<double>");
            TestType<float[]>("array<float>");
            TestType<int[]>("array<int>");
            TestType<long[]>("array<long>");
            TestType<short[]>("array<short>");

            // NOTE: unsigned types not supported in Java!
            TestType<byte[]>("array<ubyte>");
            TestType<uint[]>("array<uint>");
            TestType<ulong[]>("array<ulong>");
            TestType<ushort[]>("array<ushort>");
        }

        [Test]
        public void ToFromType_should_support_arrays_of_external_classes()
        {
            TestType<MediaUrl[]>(String.Format("array<{0}>", typeof(MediaUrl).FullName));
        }

        [Test]
        public void ToFromType_should_support_arrays_of_inner_classes()
        {
            TestType<InnerClass[]>(String.Format("array<{0}>", typeof(InnerClass).FullName));
        }

        [Test]
        public void ToFromType_should_support_arrays_of_lists()
        {
            TestType<List<int>[]>("array<list<int>>");
        }

        [Test]
        public void ToFromType_should_support_arrays_of_maps()
        {
            TestType<Dictionary<int,string>[]>("array<map<int,string>>");
        }

        [Test]
        public void ToFromType_should_support_lists_of_primitive_types()
        {
            TestType<List<sbyte>>("list<byte>");
            TestType<List<bool>>("list<boolean>");
            TestType<List<DateTime>>("list<date>");
            TestType<List<string>>("list<string>");
            TestType<List<decimal>>("list<decimal>");
            TestType<List<double>>("list<double>");
            TestType<List<float>>("list<float>");
            TestType<List<int>>("list<int>");
            TestType<List<long>>("list<long>");
            TestType<List<short>>("list<short>");

            // NOTE: unsigned types not supported in Java!
            TestType<List<byte>>("list<ubyte>");
            TestType<List<uint>>("list<uint>");
            TestType<List<ulong>>("list<ulong>");
            TestType<List<ushort>>("list<ushort>");
        }

        [Test]
        public void ToFromType_should_support_list_of_arrays()
        {
            TestType<List<int[]>>("list<array<int>>");
        }

        [Test]
        public void ToFromType_should_support_list_of_lists()
        {
            TestType<List<List<int>>>("list<list<int>>");
        }

        [Test]
        public void ToFromType_should_support_list_of_maps()
        {
            TestType<List<Dictionary<int,string>>>("list<map<int,string>>");
        }

        [Test]
        public void ToFromType_should_support_maps_of_primitive_types()
        {
            TestType<Dictionary<int, int>>("map<int,int>");
            TestType<Dictionary<string, string>>("map<string,string>");
            TestType<Dictionary<int, string>>("map<int,string>");
            TestType<Dictionary<string, int>>("map<string,int>");
        }

        [Test]
        public void ToFromType_should_support_maps_of_arrays()
        {
            TestType<Dictionary<int[], string>>("map<array<int>,string>");
            TestType<Dictionary<int, string[]>>("map<int,array<string>>");
            TestType<Dictionary<int[], string[]>>("map<array<int>,array<string>>");
        }

        [Test]
        public void ToFromType_should_support_maps_of_lists()
        {
            TestType<Dictionary<List<int>, string>>("map<list<int>,string>");
            TestType<Dictionary<int, List<string>>>("map<int,list<string>>");
            TestType<Dictionary<List<int>, List<string>>>("map<list<int>,list<string>>");
        }

        [Test]
        public void ToFromType_should_support_maps_of_maps()
        {
            TestType<Dictionary<int, Dictionary<int,string>>>("map<int,map<int,string>>");
        }

        [Test]
        public void ToFromType_should_support_maps_of_tuples()
        {
            TestType<Dictionary<int, Tuple<int, string>>>("map<int,tuple<int,string>>");
            TestType<Dictionary<int, Tuple<int, string,double>>>("map<int,tuple<int,string,double>>");
            TestType<Dictionary<int, Tuple<int, string, double, float>>>("map<int,tuple<int,string,double,float>>");
        }

        [Test]
        public void ToFromType_should_support_tuples_of_primitive_types()
        {
            // NOTE: API supports only up to Tuple<T1,T2,T3,T4>
            TestType<Tuple<int, int>>("tuple<int,int>");
            TestType<Tuple<double, double>>("tuple<double,double>");
            TestType<Tuple<int, int, int>>("tuple<int,int,int>");
            TestType<Tuple<double, double, double>>("tuple<double,double,double>");
            TestType<Tuple<int, string, double, float>>("tuple<int,string,double,float>");
        }

        [Test]
        public void ToFromType_should_support_tuples_of_arrays()
        {
            TestType<Tuple<int[], string[]>>("tuple<array<int>,array<string>>");
            TestType<Tuple<int[], string[], double[]>>("tuple<array<int>,array<string>,array<double>>");
            TestType<Tuple<int[], string[], double[], float[]>>("tuple<array<int>,array<string>,array<double>,array<float>>");
        }

        [Test]
        public void ToFromType_should_support_tuples_of_lists()
        {
            TestType<Tuple<List<int>, List<string>>>("tuple<list<int>,list<string>>");
            TestType<Tuple<List<int>, List<string>, List<double>>>("tuple<list<int>,list<string>,list<double>>");
            TestType<Tuple<List<int>, List<string>, List<double>, List<float>>>("tuple<list<int>,list<string>,list<double>,list<float>>");
        }

        [Test]
        public void ToFromType_should_support_tuples_of_maps()
        {
            TestType<Tuple<Dictionary<int,int>, Dictionary<string,string>>>("tuple<map<int,int>,map<string,string>>");
        }

        [Test]
        public void ToFromMetaObject_should_support_primitive_types()
        {
            TestSerialization(true);
            TestSerialization((sbyte)4);
            TestSerialization(new DateTime(1979, 2, 5, 14, 30, 12, 256));
            TestSerialization(1M/3M);
            TestSerialization(0.5D);
            TestSerialization("The quick brown fox jumped over the lazy dog");
            TestSerialization(0.5f);
            TestSerialization(4);
            TestSerialization((long) 44);
            TestSerialization((short) 44);

            // NOTE: unsigned types not supported in Java!
            TestSerialization((byte)4);
            TestSerialization((uint)4);
            TestSerialization((ulong)4);
            TestSerialization((ushort)4);
        }

        [Test]
        public void ToFromMetaObject_should_support_external_classes()
        {
            TestSerialization(new MediaUrl("http://www.heise.de"));
        }

        [Test]
        public void ToFromMetaObject_should_support_inner_classes()
        {
            TestSerialization(new InnerClass
            {
                Uri = "http://www.heise.de",
                Bytes = new byte[] { 0, 0, 0, 255, 0, 0, 0, 255, 0, 0, 0, 255, 255, 255, 255 }
            });
        }

        [Test]
        public void ToFromMetaObject_should_support_arrays_of_primitive_types()
        {
            TestSerialization(new sbyte[] { 1, 2, 3, 4 });
            TestSerialization(new[] { true, false, true, false });
            TestSerialization(new[] { DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromDays(1), DateTime.UtcNow + TimeSpan.FromDays(2) });
            TestSerialization(new[] { 0.1M, 1M / 3M, 10, 1000 });
            TestSerialization(new[] { 0.1, 0.5, 1, 20, 320 });
            TestSerialization("The quick brown fox jumped over the lazy dog".Split(' '));
            TestSerialization(new[] { 0.1f, 0.5f, 1f, 20f, 320f });
            TestSerialization(new[] { 1, 2, 3, 4 });
            TestSerialization(new long[] { 1, 2, 3, 4});
            TestSerialization(new short[] { 1, 2, 3, 4 });

            // NOTE: unsigned types not supported in Java!
            TestSerialization(new byte[] { 1, 2, 3, 4 });
            TestSerialization(new uint[] { 1, 2, 3, 4 });
            TestSerialization(new ulong[] { 1, 2, 3, 4 });
            TestSerialization(new ushort[] { 1, 2, 3, 4 });
        }

        [Test]
        public void ToFromMetaObject_should_support_arrays_of_external_classes()
        {
            TestSerialization(new [] { new MediaUrl("a"), new MediaUrl("b")});
        }

        [Test]
        public void ToFromMetaObject_should_support_arrays_of_inner_classes()
        {
            TestSerialization(new[] { new InnerClass { Uri = "a" }, new InnerClass { Bytes = new byte[] { 42} } });
        }

        [Test]
        public void ToFromMetaObject_should_support_arrays_of_arrays()
        {
            TestSerialization(new [] { new[] { 0, 1, 2, 3 }, new[] { 4, 5, 6, 7 } });
        }

        [Test]
        public void ToFromMetaObject_should_support_arrays_of_lists()
        {
            TestSerialization(new [] { new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7 } });
        }

        [Test]
        public void ToFromMetaObject_should_support_arrays_of_maps()
        {
            TestSerialization(new []
                {
                    new Dictionary<int, string> { {0,"a"}, {1,"b"}}
                    , new Dictionary<int, string> { {2,"c"}, {3,"d"}}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_arrays_of_tuples()
        {
            TestSerialization(new []
                {
                    new Tuple<int, string>(0,"a")
                    , new Tuple<int, string>(1,"b")
                });
        }


        [Test]
        public void ToFromMetaObject_should_support_lists_of_primitive_types()
        {
            TestSerialization(new List<bool> { true, false, true, false });
            TestSerialization(new List<sbyte> { 1, 2, 3, 4 });
            TestSerialization(new List<string>("The quick brown fox jumped over the lazy dog".Split(' ')));
            TestSerialization(new List<DateTime> { DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromDays(1), DateTime.UtcNow + TimeSpan.FromDays(2) });
            TestSerialization(new List<decimal> { 0.1M, 1M / 3M, 10, 1000 });
            TestSerialization(new List<double> { 0.1, 0.5, 1, 20, 320 });
            TestSerialization(new List<float> { 0.1f, 0.5f, 1f, 20f, 320f });
            TestSerialization(new List<int> { 1, 2, 3, 4 });
            TestSerialization(new List<long> { 1, 2, 3, 4 });
            TestSerialization(new List<short> { 1, 2, 3, 4 });

            TestSerialization(new List<byte> { 1, 2, 3, 4 });
            TestSerialization(new List<uint> { 1, 2, 3, 4 });
            TestSerialization(new List<ulong> { 1, 2, 3, 4 });
            TestSerialization(new List<ushort> { 1, 2, 3, 4 });
        }

        [Test]
        public void ToFromMetaObject_should_support_lists_of_external_classes()
        {
            TestSerialization(new List<MediaUrl> { new MediaUrl("a"), new MediaUrl("b") });
        }

        [Test]
        public void ToFromMetaObject_should_support_lists_of_inner_classes()
        {
            TestSerialization(new List<InnerClass> { new InnerClass { Uri = "a" }, new InnerClass { Bytes = new byte[] { 42 } } });
        }

        [Test]
        public void ToFromMetaObject_should_support_lists_of_arrays()
        {
            TestSerialization(new List<int[]> { new [] { 0, 1, 2, 3 }, new [] { 4, 5, 6, 7 }});
        }

        [Test]
        public void ToFromMetaObject_should_support_lists_of_lists()
        {
            TestSerialization(new List<List<int>> { new List<int> { 0, 1, 2, 3 }, new List<int> { 4, 5, 6, 7 } });
        }

        [Test]
        public void ToFromMetaObject_should_support_lists_of_maps()
        {
            TestSerialization(new List<Dictionary<int,string>>
                {
                    new Dictionary<int, string> { {0,"a"}, {1,"b"}}
                    , new Dictionary<int, string> { {2,"c"}, {3,"d"}}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_lists_of_tuples()
        {
            TestSerialization(new List<Tuple<int, string>>
                {
                    new Tuple<int, string>(0,"a")
                    , new Tuple<int, string>(1,"b")
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_maps_of_primitive_types()
        {
            TestSerialization(new Dictionary<int, int>
                {
                    {1, 9}, {2, 8}, {3, 7}, {4, 6}, {5, 5}, {6, 4}, {7, 3}, {8, 2}, {9, 1}
                });

            TestSerialization(new Dictionary<int, string>
                {
                    {1, "The"}, {2, "quick"}, {3, "brown"}, {4, "fox"}, {5, "jumped"}, 
                    {6, "over"}, {7, "the"}, {8, "lazy"}, {9, "dog"}
                });

            TestSerialization(new Dictionary<int, double>
                {
                    {1, 0.1}, {2, 0.5}, {3, 1}, {4, 2}, {5, 10}
                });

            TestSerialization(new Dictionary<string, string>
                {
                    {"Key1", "The"}, {"Key2", "quick"}, {"Key3", "brown"}, {"Key4", "fox"}, {"Key5", "jumped"}
                });

            TestSerialization(new Dictionary<string, int>
                {
                    {"Key1", 1}, {"Key2", 2}, {"Key3", 3}, {"Key4", 4}, {"Key5", 5}
                });

            TestSerialization(new Dictionary<string, double>
                {
                    {"Key1", 0.1}, {"Key2", 0.5}, {"Key3", 1}, {"Key4", 2}, {"Key5", 10}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_maps_of_external_classes()
        {
            TestSerialization(new Dictionary<int, MediaUrl>
                {
                    {0, new MediaUrl("a")}
                    , {1, new MediaUrl("b")}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_maps_of_inner_classes()
        {
            TestSerialization(new Dictionary<int, InnerClass>
                {
                    {0, new InnerClass { Uri="a"}}
                    , {1, new InnerClass { Uri="b"}}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_maps_of_arrays()
        {
            TestSerialization(new Dictionary<int, string[]>
                {
                    {0, new [] { "a", "b" }}
                    , {1, new [] { "c", "d" }}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_maps_of_lists()
        {
            TestSerialization(new Dictionary<int, List<string>>
                {
                    {0, new List<string> { "a", "b" }}
                    , {1, new List<string> { "c", "d" }}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_maps_of_maps()
        {
            TestSerialization(new Dictionary<int, Dictionary<int,string>>
                {
                    {0, new Dictionary<int, string> { {1,"a"}, {2,"b"} } }
                    , {1, new Dictionary<int, string> { {3,"c"}, {4,"d"} } }
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_maps_of_tuples()
        {
            TestSerialization(new Dictionary<int, Tuple<int, string>>
                {
                    {0, new Tuple<int, string>(1,"a")}
                    , {1, new Tuple<int, string>(2,"b")}
                });
        }

        [Test]
        public void ToFromMetaObject_should_support_tuples_of_primitive_types()
        {
            TestSerialization(new Tuple<string, string>("Hello", "you"));
            TestSerialization(new Tuple<string, int>("Hello", 1));
            TestSerialization(new Tuple<string, double>("Hello", 0.5));
            TestSerialization(new Tuple<int, string>(1, "you"));
            TestSerialization(new Tuple<int, int>(1, 2));
            TestSerialization(new Tuple<double, double>(0.5, 2.0));
            TestSerialization(new Tuple<int, int, int>(1, 2, 3));
            TestSerialization(new Tuple<double, double, double>(2.0, 4.0, 0.5));
            TestSerialization(new Tuple<int, string, double, float>(2, "Hello", 4.0, 0.5f));
        }

        [Test]
        public void ToFromMetaObject_should_support_tuples_of_external_classes()
        {
            TestSerialization(new Tuple<int, MediaUrl>(0, new MediaUrl("a")));
        }

        [Test]
        public void ToFromMetaObject_should_support_tuples_of_inner_classes()
        {
            TestSerialization(new Tuple<int, InnerClass>(0, new InnerClass { Uri="a"}));
        }

        [Test]
        public void ToFromMetaObject_should_support_tuples_of_arrays()
        {
            TestTupleOfCollections(new Tuple<int, string[]>(0, new[] { "a", "b" }));
        }

        [Test]
        public void ToFromMetaObject_should_support_tuples_of_lists()
        {
            TestTupleOfCollections(new Tuple<int, List<string>>(0, new List<string> { "a", "b" }));
        }

        [Test]
        public void ToFromMetaObject_should_support_tuples_of_maps()
        {
            TestTupleOfCollections(new Tuple<int, Dictionary<int, string>>(0, new Dictionary<int, string> { { 1, "a" }, { 2, "b" } }));
        }

        [Test]
        public void ToFromMetaObject_should_support_tuples_of_tuples()
        {
            TestSerialization(new Tuple<int, Tuple<int, string>>(0, new Tuple<int, string>(1,"a")));
        }

        // ReSharper restore InconsistentNaming

        private static void TestType(Type type, string typeName)
        {
            type.Should().Be(typeName.ToType());
            typeName.Should().BeEquivalentTo(type.ToTypeName());
        }

        private static void TestType<T>(string typeName)
        {
            TestType(typeof(T), typeName);
        }

        private static void TestSerialization<T>(T expected)
        {
            var metaObject = expected.ToMetaObject();
            Trace.WriteLine(metaObject.PrettyPrint());
            var actual = metaObject.FromMetaObject<T>();
            expected.Should().BeEquivalentTo(actual);
        }

        // FluentAssertions.Should().BeEquivalentTo(), 2.0 cannot handle Tuples of collections!
        private static void TestTupleOfCollections<T1,TCollection>(Tuple<T1,TCollection> expected)
        {
            var metaObject = expected.ToMetaObject();
            Trace.WriteLine(metaObject.PrettyPrint());
            var actual = metaObject.FromMetaObject<Tuple<T1,TCollection>>();
            expected.Item1.Should().BeEquivalentTo(actual.Item1);
            expected.Item2.Should().BeEquivalentTo(actual.Item2);
        }

        class InnerClass : IEquatable<InnerClass>
        {
            // ReSharper disable MemberCanBePrivate.Local
            public string Uri { get; set; } //= string.Empty;
            public byte[] Bytes { get; set; }
            // ReSharper restore MemberCanBePrivate.Local

            public InnerClass()
            {
                Uri = String.Empty;
                Bytes = new byte[0];
            }

            public override int GetHashCode()
            {
                return Uri.GetHashCode() ^ Bytes.GetHashCode();
            }

            public bool Equals(InnerClass other)
            {
                return Uri.Equals(other.Uri) && Bytes.SequenceEqual(other.Bytes);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is InnerClass))
                    return false;
                return Equals((InnerClass)obj);
            }
        }

        class MediaUrl : IEquatable<MediaUrl>
        {
            public readonly string Uri;

            public MediaUrl(string uri)
            {
                Uri = uri;
            }

            public bool Equals(MediaUrl other)
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
                return Equals((MediaUrl)obj);
            }

            public override int GetHashCode()
            {
                return Uri.GetHashCode();
            }
        }

    }
}
