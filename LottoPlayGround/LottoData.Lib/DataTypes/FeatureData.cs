using System;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.DataTypes
{
    public class FeatureData<T> : IData, IEquatable<FeatureData<T>>
    {
        public string FeatureName { get; }
        public T Value { get; }

        public FeatureData(string featureName, T value)
        {
            FeatureName = featureName;
            Value = value;
        }

        public bool Equals(FeatureData<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(FeatureName, other.FeatureName) && Equals(Value, other.Value);
        }

        public bool Equals(IData other)
        {
            return Equals(other as FeatureData<T>);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FeatureData<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FeatureName != null ? FeatureName.GetHashCode() : 0) * 397) ^ Value.GetHashCode();
            }
        }

        public static bool operator ==(FeatureData<T> left, FeatureData<T> right) => Equals(left, right);

        public static bool operator !=(FeatureData<T> left, FeatureData<T> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}