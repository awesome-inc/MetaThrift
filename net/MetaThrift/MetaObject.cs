using System;

namespace MetaThrift
{
    partial class MetaObject : IEquatable<MetaObject>
    {
        public static readonly MetaObject Empty = new MetaObject();

        public bool Equals(MetaObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_typeName, other._typeName) && string.Equals(_data, other._data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MetaObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyFieldInGetHashCode
                return ((_typeName != null ? _typeName.GetHashCode() : 0)*397) ^ (_data != null ? _data.GetHashCode() : 0);
                // ReSharper restore NonReadonlyFieldInGetHashCode
            }
        }
    }
}
