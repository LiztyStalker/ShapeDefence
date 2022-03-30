namespace System.Numerics
{
    using UnityEngine;
    //using Unity.Mathematics;

    [System.Serializable]
    public struct BigDecimal : IComparable, IComparable<BigDecimal>, IConvertible, IEquatable<BigDecimal>, IFormattable
    {


        //private static Dictionary<byte, int> _powDic = new Dictionary<byte, int>();

        private const byte MAXIMUM_DIVIDE_DECIMAL_POINT = 10;

        [SerializeField] private BigInteger _value;

        [SerializeField] private byte _decimalPoint;
        public byte DecimalPoint => _decimalPoint;

        public bool IsZero => (_value.IsZero && _decimalPoint == 0);

        public BigInteger Value { get => _value / BigInteger.Pow(10, _decimalPoint); set => _value = value; }

        public BigDecimal(int value)
        {
            _decimalPoint = 0;
            _value = ConvertToBigInteger((decimal)value);
        }
        public BigDecimal(double value)
        {
            _decimalPoint = 0;
            _value = ConvertToBigInteger((decimal)value);
        }
        public BigDecimal(float value)
        {
            _decimalPoint = 0;
            _value = ConvertToBigInteger((decimal)value);
        }
        public BigDecimal(decimal value)
        {
            _decimalPoint = 0;
            _value = ConvertToBigInteger(value);
        }
        public BigDecimal(short value)
        {
            _decimalPoint = 0;
            _value = ConvertToBigInteger(value);
        }
        public BigDecimal(long value)
        {
            _decimalPoint = 0;
            _value = ConvertToBigInteger(value);
        }

        public BigDecimal(string value)
        {
            var split = value.Split('.');
            if (split.Length == 1)
            {
                _value = BigInteger.Parse(value);
                _decimalPoint = 0;
            }
            else if (split.Length == 2)
            {
                _value = BigInteger.Parse(value);
                _decimalPoint = (byte)split[1].Length;
            }
            else
            {
                throw new Exception($"{value}는 BigDecimal에 대응할 수 없습니다");
            }
        }

        public BigDecimal(BigDecimal bigdec)
        {
            _value = bigdec._value;
            _decimalPoint = bigdec._decimalPoint;
        }

        public BigDecimal(BigInteger value, int decimalPoint = 0)
        {
            _value = value;
            _decimalPoint = (byte)decimalPoint;
        }

        public void Clear()
        {
            _value = 0;
        }


        public decimal GetDecimalValue() => GetDecimalValue(_value, _decimalPoint);

        private static decimal GetDecimalValue(BigInteger bigint, byte exponent)
        {
            //오버플로우 버그
            return ((decimal)bigint) * (decimal)Mathf.Pow(0.1f, (int)exponent);
        }

        public string ToString(string format) {

            if (string.IsNullOrEmpty(format)) 
                return ToString();
            return string.Format(format, GetDecimalValue());
        }

        public override string ToString() => Utility.Number.NumberDataUtility.GetSummaryValue(this, 3, "", "K", "M", "G", "T", "P", "Z", "Y");

        private BigInteger ConvertToBigInteger(decimal value)
        {
            CalculateDecimalPoint((decimal)value, out _decimalPoint, out BigInteger integer);
            return integer;
        }

        public static void CalculateDecimalPoint(decimal value, out byte decimalPoint, out BigInteger integer)
        {
            decimal nowValue = value;
            decimalPoint = 0;
            //Debug.Log(nowValue);
            while (true)
            {
                if (nowValue % 1 == 0)
                {
                    break;
                }
                nowValue *= 10;
                decimalPoint++;
            }
            integer = new BigInteger(nowValue);
        }

        public void SetValue(BigInteger value, byte decimalPoint)
        {
            _value = value;
            _decimalPoint = decimalPoint;
        }



        private BigDecimal(BigInteger value, byte decimalPoint)
        {
            var result = value;
            var point = decimalPoint;

            while (true)
            {
                if(point > 0 && result % 10 == 0)
                {
                    result /= 10;
                    point--;
                }
                else
                {
                    break;
                }
            }
//            Debug.Log(result + " " + point);
            _value = result;
            _decimalPoint = point;
        }

        private BigDecimal MatchingDecimalPoint(byte decimalPoint)
        {
            var data = new BigDecimal();
            var point = _decimalPoint;
            byte gapDecimalPoint = 0;
            if (point > decimalPoint)
                gapDecimalPoint = (byte)(point - decimalPoint);
            else if (point < decimalPoint)
                gapDecimalPoint = (byte)(decimalPoint - point);


            int pow = (int)Mathf.Pow(10, gapDecimalPoint);
            //Debug.Log((_value * pow) + " " + (point + gapDecimalPoint));

            data.SetValue(_value * pow, (byte)(point + gapDecimalPoint));
            return data;
        }


        public static BigDecimal operator +(BigDecimal a, BigDecimal b)
        {
            MatchingDecimalPoint(ref a, ref b);
            //Debug.Log(a._value + " " + a._decimalPoint + " " + b._value + " " + b._decimalPoint);
            return new BigDecimal(a._value + b._value, a._decimalPoint);
        }
        public static BigDecimal operator -(BigDecimal a, BigDecimal b)
        {
            MatchingDecimalPoint(ref a, ref b);
            return new BigDecimal(a._value - b._value, a._decimalPoint);
        }

        public static BigDecimal operator *(BigDecimal a, BigDecimal b)
        {
            return new BigDecimal(BigInteger.Multiply(a._value, b._value), (byte)(a._decimalPoint + b._decimalPoint));
        }
        
        public static BigDecimal operator /(BigDecimal a, BigDecimal b)
        {
            if (b._decimalPoint == 0 && b._value == 0)
            {
                throw new System.DivideByZeroException();
            }

            //소숫점 자리수
            MatchingDecimalPoint(ref a, ref b);

            var result = new BigDecimal();
            var nowValue = a._value;
            var decimalPoint = 0;
            while (true)
            {
                result += new BigDecimal(nowValue / b._value, (byte)decimalPoint);
                var moduler = nowValue % b._value;

                if (moduler == 0 || decimalPoint >= MAXIMUM_DIVIDE_DECIMAL_POINT)
                    break;

                decimalPoint++;
                nowValue = moduler * 10;
            }
            return result;
        }


        public static BigDecimal operator %(BigDecimal a, BigDecimal b)
        {
            MatchingDecimalPoint(ref a, ref b);
            return new BigDecimal(a._value % b._value, a._decimalPoint);
        }

        public static BigDecimal operator ++(BigDecimal value)
        {
            if (value._decimalPoint > 0)
            {
                value._value += BigInteger.Pow(10, value._decimalPoint);
            }
            else
            {
                value._value++;
            }
            return value;
        }
        public static BigDecimal operator --(BigDecimal value)
        {
            if (value._decimalPoint > 0)
            {
                value._value -= BigInteger.Pow(10, value._decimalPoint);
            }
            else
            {
                value._value--;
            }
            return value;
        }

        //public static BigInteger operator &(BigInteger left, BigInteger right);
        //public static BigInteger operator |(BigInteger left, BigInteger right);
        //public static BigInteger operator ^(BigInteger left, BigInteger right);
        //public static BigInteger operator <<(BigInteger value, int shift);
        //public static BigInteger operator >>(BigInteger value, int shift);


        public static bool operator ==(BigDecimal a, BigDecimal b) => (a._value == b._value && a._decimalPoint == b._decimalPoint);
        public static bool operator !=(BigDecimal a, BigDecimal b) => (a._value != b._value || a._decimalPoint != b._decimalPoint);

        //소수점 통일 후 비교
        public static bool operator >(BigDecimal a, BigDecimal b)
        {
            MatchingDecimalPoint(ref a, ref b);
            return (BigInteger.Compare(a._value, b._value) > 0);
        }
        public static bool operator <(BigDecimal a, BigDecimal b)
        {
            MatchingDecimalPoint(ref a, ref b);
            return (BigInteger.Compare(a._value, b._value) < 0);
        }
        public static bool operator >=(BigDecimal a, BigDecimal b)
        {
            MatchingDecimalPoint(ref a, ref b);
            return (BigInteger.Compare(a._value, b._value) >= 0);
        }
        public static bool operator <=(BigDecimal a, BigDecimal b)
        {
            MatchingDecimalPoint(ref a, ref b);
            return (BigInteger.Compare(a._value, b._value) <= 0);
        }


        public static implicit operator BigDecimal(long value) => new BigDecimal(value);
        public static implicit operator BigDecimal(int value) => new BigDecimal(value);
        public static implicit operator BigDecimal(short value) => new BigDecimal(value);
        public static implicit operator BigDecimal(float value) => new BigDecimal(value);
        public static implicit operator BigDecimal(double value) => new BigDecimal(value);
        public static implicit operator BigDecimal(decimal value) => new BigDecimal(value);
//        public static implicit operator BigDecimal(BigInteger value) => new BigDecimal(value);


        //public static explicit operator byte(BigDecimal value);
        public static explicit operator decimal(BigDecimal value) => GetDecimalValue(value._value, value._decimalPoint);
        public static explicit operator double(BigDecimal value) => (double)GetDecimalValue(value._value, value._decimalPoint);
        public static explicit operator float(BigDecimal value) => (float)GetDecimalValue(value._value, value._decimalPoint);
        public static explicit operator short(BigDecimal value) => (short)GetDecimalValue(value._value, value._decimalPoint);
        public static explicit operator long(BigDecimal value) => (long)GetDecimalValue(value._value, value._decimalPoint);
        public static explicit operator int(BigDecimal value) => (int)GetDecimalValue(value._value, value._decimalPoint);
        //[CLSCompliant(false)]
        //public static explicit operator sbyte(BigDecimal value);
        //[CLSCompliant(false)]
        //public static explicit operator ushort(BigDecimal value);
        //[CLSCompliant(false)]
        //public static explicit operator uint(BigDecimal value);
        //[CLSCompliant(false)]
        //public static explicit operator ulong(BigDecimal value);
        //public static explicit operator BigDecimal(float value);
        //public static explicit operator float(BigDecimal value);


        private static void MatchingDecimalPoint(ref BigDecimal a, ref BigDecimal b)
        {
            if (b._decimalPoint > a._decimalPoint)
            {
                a = a.MatchingDecimalPoint(b._decimalPoint);
            }
            else if (a._decimalPoint > b._decimalPoint)
            {
                b = b.MatchingDecimalPoint(a._decimalPoint);
            }
        }


        public static BigDecimal Pow(BigDecimal value, int exponent)
        {
            var bigint = BigInteger.Pow(value._value, exponent);
            var dPoint = value._decimalPoint * exponent;
            return new BigDecimal(bigint, dPoint);
        }

        public override bool Equals(object obj)
        {
            var bigDec = (BigDecimal)obj;
            return (_value == bigDec._value && _decimalPoint == bigDec._decimalPoint);
        }

        public override int GetHashCode() => base.GetHashCode();

        public int CompareTo(object obj) => BigInteger.Compare(_value, (BigInteger)obj);

        public int CompareTo(BigDecimal other) => BigInteger.Compare(_value, other._value);

        public bool Equals(BigDecimal other) => _value == other._value && _decimalPoint == other._decimalPoint;

        public TypeCode GetTypeCode() => TypeCode.Object;

        public bool ToBoolean(IFormatProvider provider) => Convert.ToBoolean(_value, provider);

        public byte ToByte(IFormatProvider provider) => Convert.ToByte(_value, provider);

        public char ToChar(IFormatProvider provider) => Convert.ToChar(_value, provider);

        public DateTime ToDateTime(IFormatProvider provider) => Convert.ToDateTime(_value, provider);

        public decimal ToDecimal(IFormatProvider provider) => Convert.ToDecimal(_value, provider);

        public double ToDouble(IFormatProvider provider) => Convert.ToDouble(_value, provider);

        public short ToInt16(IFormatProvider provider) => Convert.ToInt16(_value, provider);

        public int ToInt32(IFormatProvider provider) => Convert.ToInt32(_value, provider);

        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(_value, provider);

        public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(_value, provider);

        public float ToSingle(IFormatProvider provider) => Convert.ToSingle(_value, provider);

        public string ToString(IFormatProvider provider) => Convert.ToString(_value, provider);

        public object ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(_value, conversionType, provider);

        public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(_value, provider);

        public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(_value, provider);

        public ulong ToUInt64(IFormatProvider provider) => (ulong)Convert.ToInt64(_value, provider);

        public string ToString(string format, IFormatProvider formatProvider) => _value.ToString();

    }
}