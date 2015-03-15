using System;

namespace MetaThrift
{
    partial class MetaOperation : IEquatable<MetaOperation>
    {
        public bool Equals(MetaOperation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_name, other._name) 
                && string.Equals(_inputTypeName, other._inputTypeName) 
                && string.Equals(_outputTypeName, other._outputTypeName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MetaOperation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyFieldInGetHashCode
                int hashCode = (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_inputTypeName != null ? _inputTypeName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_outputTypeName != null ? _outputTypeName.GetHashCode() : 0);
                // ReSharper restore NonReadonlyFieldInGetHashCode
                return hashCode;
            }
        }
    }
}