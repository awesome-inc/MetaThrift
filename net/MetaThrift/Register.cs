using System;

namespace MetaThrift
{
    public static class Register
    {
        public static void Type<T>(string typeName) { Type(typeof(T), typeName); }
        public static void Type(Type type, String typeName) { SerializationHelper.RegisterType(type, typeName); }
    }
}