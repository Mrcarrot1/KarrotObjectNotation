using System;

namespace KarrotObjectNotation
{
    public class KONValue<T> : KONValue
    {
        public new Type valueType = typeof(T);

        public new T Value { get; set; }

        public KONValue(T value)
        {
            Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
        public static explicit operator string(KONValue<T> value)
        {
            return value.Value.ToString();
        }
        /*public static implicit operator T(KONValue<T> value)        
        {
            return value.Value;
        }*/
    }
    public class KONValue : IKONValue
    {
        public object Value { get; }
        public Type valueType;

        public static implicit operator KONValue(string value)
        {
            return new KONValue<string>(value);
        }
    }
}