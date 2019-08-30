using System;
using System.Collections.Generic;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.DataTypes
{

    public class BaseData<T>:IData, IEquatable<BaseData<T>>
    {
        public T Data { get; }
        public string Field { get; }

        public BaseData(T data,string field)
        {
            Data = data;
            Field = field;
        }

        public bool Equals(BaseData<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Data, other.Data);
        }

        public bool Equals(IData other)
        {
            return Equals(other as BaseData<T>);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseData<T>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Data);
        }

        public static bool operator ==(BaseData<T> left, BaseData<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseData<T> left, BaseData<T> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Field;
        }
    }
}
