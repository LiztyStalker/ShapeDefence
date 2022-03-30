namespace Utility.Number
{
    using System.Collections.Generic;
    using System.Numerics;
    using System.Text;

    public class NumberDataUtility
    {
        private const int NUMBER_ALPHABET = 'Z' - 'A'; //26

        public static T Create<T>() where T : INumberData
        {
            return System.Activator.CreateInstance<T>();
        }

        public static INumberData Create(System.Type type)
        {
            return (INumberData)System.Activator.CreateInstance(type);
        }


        /// <summary>
        /// <br>���� ����</br>
        /// </summary>
        /// <param name="startValue">�ʱⰪ</param>
        /// <param name="nowValue">������</param>
        /// <param name="rate">������</param>
        /// <param name="length">�Ⱓ</param>
        /// <returns></returns>
        public static BigDecimal GetCompoundInterest(BigDecimal startValue, float nowValue = 1, float rate = 0.1f, int length = 1)
        {
            var exponent = length;
            var nv = nowValue;
            var rt = rate;
            var value = startValue * BigDecimal.Pow(nv + rt, exponent);
            return value;
        }

        /// <summary>
        /// �ܸ� ����
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="nowValue"></param>
        /// <param name="rate"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        //public static BigDecimal GetIsolationInterest(BigDecimal startValue, float nowValue = 1, int length = 1)
        //{
        //    var value = startValue + nowValue * length;
        //    return value;
        //}


        /// <summary>
        /// �ܸ� ����
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="increaseValue"></param>
        /// <param name="increaseRate"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static BigDecimal GetIsolationInterest(BigDecimal startValue, float increaseValue = 1, float increaseRate = 0.1f, int length = 1)
        {
            var value = startValue + increaseValue * (float)length;
            value += value * increaseRate * (float)length;
            return value;
        }

        /// <summary>
        /// �ܸ� ����
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="increaseValue"></param>
        /// <param name="increaseRate"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static float GetIsolationInterest(float startValue, float increaseValue = 1, float increaseRate = 0.1f, int length = 1)
        {
            var value = startValue + increaseValue * (float)length;
            value += value * increaseRate * (float)length;
            return value;
        }


        public static string GetSummaryValue(BigDecimal bigdec, int digitLength = 3, params string[] digitUnits)
        {
            var str = bigdec.Value.ToString();
            int capacity = GetDigitCapacity(bigdec);

            if (capacity > 0)
            {
                if (capacity < digitUnits.Length)
                {
                    //�ڸ�������
                    string digit = digitUnits[capacity];

                    //�Ҽ��� ����
                    int dot = (str.Length % digitLength);
                    int length = (dot == 0) ? digitLength : dot;

                    //���ڸ� ���ڸ� ���� ����
                    var str1 = str.Substring(0, length);
                    var str2 = str.Substring(length, (digitLength + 1) - length);

                    //���
                    return $"{str1}.{str2}{digit}";
                }
                else
                {
                    //������ A-Z ����
                    return GetSummaryValue(bigdec, digitLength, digitUnits.Length);
                }
            }
            //���
            return str;
        }


        public static string GetSummaryValue(BigDecimal bigdec, int digitLength = 3, int offsetDigitLength = 0)
        {
            var str = bigdec.Value.ToString();
            //Debug.Log(str);
            int capacity = GetDigitCapacity(bigdec);

            if (capacity > 0)
            {
                //�ڸ�������
                string digit = GetDigit(capacity);

                //�Ҽ��� ����
                int dot = (str.Length % digitLength);
                int length = (dot == 0) ? digitLength : dot;

                //���ڸ� ���ڸ�
                var str1 = str.Substring(0, length);
                var str2 = str.Substring(length, (digitLength + 1) - length);
                return $"{str1}.{str2}{digit}";
            }
            //���
            return str;
        }

        private static int GetDigitCapacity(BigDecimal bigInt)
        {
            var value = bigInt.Value;
            var capacity = 0;
            while (true)
            {
                value /= 1000;
                if (value == 0)
                {
                    break;
                }
                capacity++;
            }
            return capacity;
        }

        public static string GetDigitValue(BigDecimal bigInt)
        {
            Stack<string> stack = new Stack<string>();
            var value = bigInt.Value;
            while (true)
            {
                var digit = GetDigit(stack.Count);
                var str = value % 1000;
                stack.Push(string.Format("{0:d3}{1}", str, digit));
                value /= 1000;
                if (value == 0)
                    break;
            }

            StringBuilder builder = new StringBuilder();

            while (true)
            {
                builder.Append(stack.Pop());
                if (stack.Count == 0)
                    break;
            }
            return builder.ToString();
        }


        private static string GetDigit(int capacity)
        {
            if (capacity == 0)
            {
                return "";
            }

            capacity--;

            StringBuilder builder = new StringBuilder();

            var digit = capacity;

            while (true)
            {
                if (digit / NUMBER_ALPHABET == 0)
                {
                    builder.Append(GetAlphabet(digit));
                    break;
                }
                else
                {
                    digit = (digit / NUMBER_ALPHABET) - 1;
                    builder.Append(GetAlphabet(digit % NUMBER_ALPHABET));
                }
            }
            return builder.ToString();
        }

        private static string GetAlphabet(int capacity) => char.ConvertFromUtf32('A' + capacity).ToString();

    }
}