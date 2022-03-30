namespace Utility.Number
{
    using System.Collections.Generic;
    using System.Numerics;
    using System.Text;

    public class NumberDataUtility
    {
        private const int NUMBER_ALPHABET = 'Z' - 'A' + 1; //26

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

        /// <summary>
        /// ��� �� ���
        /// </summary>
        /// <param name="bigdec">��</param>
        /// <param name="digitLength">�ڸ��� ����</param>
        /// <param name="digitUnits">�ڸ��� ����</param>
        /// <returns></returns>
        public static string GetSummaryValue(BigDecimal bigdec, int digitLength = 3, params string[] digitUnits)
        {
            var str = bigdec.Value.ToString();
            int capacity = GetDigitCapacity(bigdec, digitLength);
            //UnityEngine.Debug.Log(capacity);


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
                    //������ �ڸ����� ������ AA-ZZ �ڸ��� ���
                    return GetSummaryValue(bigdec, digitLength, NUMBER_ALPHABET - digitUnits.Length);
                }
            }
            //���
            return str;
        }

        /// <summary>
        /// ��� �� ���
        /// </summary>
        /// <param name="bigdec">��</param>
        /// <param name="digitLength">�ڸ��� ����</param>
        /// <param name="offsetDigitLength">�ڸ��� ���� ������</param>
        /// <returns></returns>
        public static string GetSummaryValue(BigDecimal bigdec, int digitLength = 3, int offsetDigitLength = 0)
        {
            var str = bigdec.Value.ToString();
            int capacity = GetDigitCapacity(bigdec, digitLength) + offsetDigitLength;
            //UnityEngine.Debug.Log(capacity);

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

        private static int GetDigitCapacity(BigDecimal bigdec, int digitLength)
        {
            var value = bigdec.Value;
            var capacity = 0;
            while (true)
            {
                value /= (int)UnityEngine.Mathf.Pow(10, digitLength);
                if (value == 0)
                {
                    break;
                }
                capacity++;
            }
            return capacity;
        }

        private static string GetDigit(int capacity)
        {
            StringBuilder builder = new StringBuilder();

            int digit = capacity;
            int mod = digit;

            while (true)
            {
                //UnityEngine.Debug.Log(digit);
                //1�ڸ���
                if (digit / NUMBER_ALPHABET == 0)
                {
                    builder.Append(GetAlphabet(mod));
                    break;
                }
                //2�ڸ���
                else
                {
                    int alpha = digit;
                    while (alpha / NUMBER_ALPHABET != 0)
                    {
                        alpha /= NUMBER_ALPHABET;
                    }
                    builder.Append(GetAlphabet(alpha - 1));
                    mod = digit % NUMBER_ALPHABET;
                    digit /= NUMBER_ALPHABET;
                }
            }
            return builder.ToString();
        }

        private static string GetAlphabet(int capacity) => char.ConvertFromUtf32('A' + capacity).ToString();

    }
}