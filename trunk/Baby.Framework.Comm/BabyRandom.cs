using System;

namespace Baby.Framework.Comm
{
    /// <summary>
    /// 随机辅助类
    /// </summary>
    public class BabyRandom
    {
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="randomLength">随机字符串长度</param>
        /// <param name="createType">生成的字符串类型</param>
        /// <returns>随机字符串</returns>
        public static string CreateRandomString(int randomLength = 4,RandomCreateType createType = (RandomCreateType.Number & RandomCreateType.Letter))
        {
            string randomStr = string.Empty;
            Random random = new Random();
            for (int i = 0; i < randomLength; i++) {
                randomStr += GetCharByRandomCreateType(random.Next(), createType);
            }
            return randomStr;
        }

        /// <summary>
        /// 根据数字和生成的字符串类型返回随机字符
        /// </summary>
        /// <param name="number">随机数字</param>
        /// <param name="createType">生成的字符串类型</param>
        /// <returns>随机字符</returns>
        private static char GetCharByRandomCreateType(int number,RandomCreateType createType)
        {
            char randomChar = default(char);
            switch (createType) {
                case RandomCreateType.Letter:
                    randomChar = (char) ('0' + (char) (number%10));
                    break;
                case RandomCreateType.Number:
                    randomChar = (char) ('A' + (char) (number%26));
                    break;
                case RandomCreateType.Number & RandomCreateType.Letter:
                    randomChar = (char)(number % 2 == 0 ? ('0' + (char)(number % 10)) : ('A' + (char)(number % 26)));
                    break;
                default:
                    randomChar = (char)('0' + (char)(number % 10));
                    break;
            }
            return randomChar;
        }

        [Flags]
        public enum RandomCreateType
        {
            Number = 1,
            Letter = 2
        }
    }
}
