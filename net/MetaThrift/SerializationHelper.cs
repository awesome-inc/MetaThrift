using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MetaThrift
{
    internal static class SerializationHelper
    {
        private static readonly Dictionary<string, Type> WellKnownTypes;
        private static readonly Dictionary<Type, string> WellKnownTypeNames;

        static readonly Dictionary<string, Type> RegisteredTypes = new Dictionary<string, Type>();
        static readonly Dictionary<Type, string> RegisteredTypeNames = new Dictionary<Type, string>();

        private static readonly Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        static SerializationHelper()
        {
            WellKnownTypes = new Dictionary<string, Type>
            {
                {"boolean", typeof (Boolean)}
                , {"byte", typeof (SByte)}
                , {"date", typeof (DateTime)}
                , {"decimal", typeof (Decimal)}
                , {"double", typeof (Double)}
                , {"string", typeof (String)}
                , {"float", typeof (Single)}
                , {"int", typeof (Int32)}
                , {"long", typeof (Int64)}
                , {"short", typeof (Int16)}
                // NOTE: unsigned types not supported in Java!
                , {"ubyte", typeof (Byte)}
                , {"uint", typeof (UInt32)}
                , {"ulong", typeof (UInt64)}
                , {"ushort", typeof (UInt16)}
            };

            WellKnownTypeNames = WellKnownTypes.ToDictionary(item => item.Value, item => item.Key);
        }

        internal static Type ToType(this string typeName)
        {
            if (String.IsNullOrEmpty(typeName)) return typeof(void);

            Type type;
            if (WellKnownTypes.TryGetValue(typeName, out type)) return type;

            if (RegisteredTypes.TryGetValue(typeName, out type)) return type;

            type = ToGenericType(typeName);
            if (type != null) return type;

            // check for well-known .NET types
            type = Type.GetType(typeName, false, true); // do not throw on error, ignore case!
            if (type != null) // found
                return type;

            // check cache for user-defined types (.NET, XSD-generated)
            if (TypeCache.TryGetValue(typeName, out type)) // found
                return type;

            // check loaded domains for user-defined typess
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName, false, true);
                if (type == null) continue;
                TypeCache.Add(typeName, type); // add to cache
                return type;
            }

            // not found. give up.
            return null;
        }

        internal static string ToTypeName(this Type type)
        {
            if (type == typeof (void)) return String.Empty;

            String typeName;
            if (WellKnownTypeNames.TryGetValue(type, out typeName)) return typeName;

            if (RegisteredTypeNames.TryGetValue(type, out typeName)) return typeName;

            typeName = ToGenericTypeName(type);
            if (!String.IsNullOrEmpty(typeName)) return typeName;

            return type.FullName;
        }

        internal static string ToJson(this object value, Type type)
        {
            if (value != null)
            {
                var valuesType = value.GetType();
                if (valuesType == type) return JsonConvert.SerializeObject(Convert.ChangeType(value, type));

                throw new System.ArgumentException(
                    String.Format("value should be of type {0} but is {1}", type.Name, valuesType.Name), "type");
            }

            if (type.AllowsNull()) return String.Empty;
            throw new System.ArgumentException(String.Format("null value not allowed for {0}", type.Name), "type");
        }

        internal static object FromJson(this string value, Type type)
        {
            var isNull = String.IsNullOrEmpty(value);
            if (!isNull) return JsonConvert.DeserializeObject(value, type);
            if (type.AllowsNull()) return null;
            throw new System.ArgumentException(String.Format("null value not allowed for {0}", type.Name), "type");
        }

        internal static TValue FromJson<TValue>(this string value)
        {
            return JsonConvert.DeserializeObject<TValue>(value);
        }

        static bool AllowsNull(this Type type)
        {
            return !type.IsValueType || type == typeof(void);            
        }

        private const string ArrayPrefix = "array<";
        private const string ListPrefix = "list<";
        private const string MapPrefix = "map<";
        private const string TuplePrefix = "tuple<";

        private static Type ToGenericType(string typeName)
        {
            if (typeName.StartsWith(ArrayPrefix))
                return GetArrayType(typeName);

            if (typeName.StartsWith(ListPrefix))
                return GetListType(typeName);

            if (typeName.StartsWith(MapPrefix))
                return GetMapType(typeName);

            // NOTE: API supports only up to Tuple<T1,T2,T3,T4>
            if (typeName.StartsWith(TuplePrefix))
                return GetTupleType(typeName);

            return null;
        }

        private static Type GetArrayType(string metaTypeName)
        {
            var metaTypeArgument = metaTypeName.Substring(ArrayPrefix.Length, metaTypeName.Length - ArrayPrefix.Length - 1);
            var argumentType = metaTypeArgument.ToType();
            var arrayTypeName = String.Format("{0}[], {1}", argumentType.FullName, argumentType.Assembly.FullName);
            return Type.GetType(arrayTypeName);
        }

        private static Type GetListType(string metaTypeName)
        {
            var metaTypeArgument = metaTypeName.Substring(ListPrefix.Length, metaTypeName.Length - ListPrefix.Length - 1);
            var argumentType = metaTypeArgument.ToType();
            var listTypeName = String.Format("System.Collections.Generic.List`1[[{0}]], mscorlib", argumentType.AssemblyQualifiedName);
            return Type.GetType(listTypeName);
        }

        private static Type GetMapType(string metaTypeName)
        {
            var metaTypeArguments = metaTypeName.Substring(MapPrefix.Length, metaTypeName.Length - MapPrefix.Length - 1).SplitArgs();
            var typeArgumentNames = metaTypeArguments.Select(ToType).Select(type => type.AssemblyQualifiedName).ToList();
            var mapTypeName = String.Format("System.Collections.Generic.Dictionary`2[[{0}]]", String.Join("],[", typeArgumentNames));
            return Type.GetType(mapTypeName);
        }

        private static Type GetTupleType(string metaTypeName)
        {
            var metaTypeArguments = metaTypeName.Substring(TuplePrefix.Length, metaTypeName.Length - TuplePrefix.Length - 1).SplitArgs();
            var typeArgumentNames = metaTypeArguments.Select(ToType).Select(type => type.AssemblyQualifiedName).ToList();
            var tupleTypeName = String.Format("System.Tuple`{0}[[{1}]]", typeArgumentNames.Count, String.Join("],[", typeArgumentNames));
            return Type.GetType(tupleTypeName);
        }

        private static IEnumerable<string> SplitArgs(this string str)
        {
            var args = new List<string>();
            if (!SplitArgs(str, args))
                args.Add(str);
            return args.ToArray();
        }

        private static bool SplitArgs(string str, ICollection<string> args)
        {
            int open = 0;
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '<': open++; break;
                    case ',':
                        if (open == 0 && i > 0)
                        {
                            var left = str.Substring(0, i);
                            args.Add(left);
                            var right = str.Substring(i + 1);
                            if (!SplitArgs(right, args))
                                args.Add(right);
                            return true;
                        }
                        break;
                    case '>': open--; break;
                }
            }
            return false;
        }

        private static readonly Type[] TupleTypes = 
        { typeof(Tuple<,>), typeof(Tuple<,,>), typeof(Tuple<,,,>), typeof(Tuple<,,,,>) };

        private static string ToGenericTypeName(Type type)
        {
            if (type.IsArray)
                return String.Concat(ArrayPrefix, type.GetElementType().ToTypeName(), ">");

            if (!type.IsGenericType) return String.Empty;
            var genericType = type.GetGenericTypeDefinition();
            var argTypeNames = String.Join(",", type.GetGenericArguments().Select(ToTypeName));

            if (genericType == typeof(List<>))
                return String.Concat(ListPrefix, argTypeNames, ">");
            
            if (genericType == typeof(Dictionary<,>))
                return String.Concat(MapPrefix, argTypeNames, ">");
            
            if (TupleTypes.Contains(genericType))
                return String.Concat(TuplePrefix, argTypeNames, ">");
            
            return String.Empty;
        }

        internal static void RegisterType(Type type, string typeName)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (String.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException("typeName");

            if (WellKnownTypes.ContainsKey(typeName))
                throw new System.ArgumentException("A well known type name must not be used when registering custom types.", "typeName");
            if (WellKnownTypeNames.ContainsKey(type))
                throw new System.ArgumentException("A well known type must not be used when registering custom types.", "type");
            if (RegisteredTypes.ContainsKey(typeName))
                throw new System.ArgumentException("A type with the specified name has already been registered.", "typeName");
            if (RegisteredTypeNames.ContainsKey(type))
                throw new System.ArgumentException("The specified type has already been registered.", "typeName");

            RegisteredTypes.Add(typeName, type);
            RegisteredTypeNames.Add(type, typeName);
        }

        internal static void UnregisterType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            string typeName;
            if (!RegisteredTypeNames.TryGetValue(type, out typeName))
                throw new System.ArgumentException("The specified type has not been registered as a custom type.");
            RegisteredTypeNames.Remove(type);
            RegisteredTypes.Remove(typeName);
        }
    }
}
