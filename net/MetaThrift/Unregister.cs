using System;

namespace MetaThrift
{
    public static class Unregister
    {
        public static void Type<T>() { Type(typeof(T)); }
        public static void Type(Type type) { SerializationHelper.UnregisterType(type); }
    }
}