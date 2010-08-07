using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Baby.Framework.Comm
{
    public class EncryptionHelper
    {
        #region 非对称加密 不可逆
        /// <summary>
        /// SHA1加密为byte数组
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <returns>加密过的byte数组</returns>
        public static byte[] SHA1_Encode(string str)
        {
            SHA1 md5 = new SHA1CryptoServiceProvider();
            byte[] pass_type = md5.ComputeHash(ConvertStringToByteArray(str));

            return pass_type;
        }

        /// <summary>
        /// SHA1加密为指定编码格式的byte数组
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <param name="encoding">指定的编码格式</param>
        /// <returns>加密过的byte数组</returns>
        public static byte[] SHA1_Encode(string str, Encoding encoding)
        {
            SHA1 md5 = new SHA1CryptoServiceProvider();
            byte[] pass_type = md5.ComputeHash(ConvertStringToByteArray(str, encoding));

            return pass_type;
        }

        /// <summary>
        /// MD5加密为byte数组
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <returns>加密过的byte数组</returns>
        public static byte[] MD5_Encode(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] encodePass = md5.ComputeHash(ConvertStringToByteArray(str));

            return encodePass;
        }

        /// <summary>
        /// MD5加密为byte数组
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <param name="encoding">指定的编码格式</param>
        /// <returns>加密过的byte数组</returns>
        public static byte[] MD5_Encode(string str, Encoding encoding)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] encodePass = md5.ComputeHash(ConvertStringToByteArray(str, encoding));

            return encodePass;
        }

        /// <summary>
        /// MD5加密为字符串
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5ToString(string str)
        {
            return BitConverter.ToString(MD5_Encode(str));
        }

        /// <summary>
        /// MD5加密为指定编码格式的字符串
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <param name="encoding">指定的编码格式</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5ToString(string str, Encoding encoding)
        {
            return BitConverter.ToString(MD5_Encode(str, encoding));
        }

        /// <summary>
        /// SHA1加密为字符串
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <returns>加密过的string</returns>
        public static string SHA1ToString(string str)
        {
            return BitConverter.ToString(SHA1_Encode(str));
        }

        /// <summary>
        /// SHA1加密为指定编码格式的字符串
        /// </summary>
        /// <param name="str">欲加密的字符串</param>
        /// <param name="encoding">指定的编码格式</param>
        /// <returns>加密过的字符串</returns>
        public static string SHA1ToString(string str, Encoding encoding)
        {
            return BitConverter.ToString(SHA1_Encode(str, encoding));
        }
        /// <summary>
        /// 将字符串转化为byte数组
        /// </summary>
        /// <param name="s">欲转换的string</param>
        /// <returns>转化后的Byte数组</returns>
        public static byte[] ConvertStringToByteArray(String s)
        {
            return (new UnicodeEncoding()).GetBytes(s);
        }

        /// <summary>
        /// 将字符串转化为指定编码的byte数组
        /// </summary>
        /// <param name="str">欲转化的字符串</param>
        /// <param name="encoding">指定的编码</param>
        /// <returns>转化后的byte数组</returns>
        public static byte[] ConvertStringToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        #endregion

        #region 对称加密

        /// <summary>
        /// 对称加密使用的密钥
        /// </summary>
        public static readonly string sDes_key = "happyday";
        
        #region 对称加密方法

        /// <summary>
        /// 对称加密方法
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>被加密过的字符串</returns>
        public static string SymmetricDesEncode(string pToEncrypt, string sKey)
        {
            string encodeStr = string.Empty;
            try
            {
                //先转换
                //pToEncrypt = HttpContext.Current.Server.UrlEncode(pToEncrypt);

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //把字符串放到byte数组中

                //使用当前系统的编码,不行则改成utf-8
                byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

                //建立加密对象的密钥和偏移量

                //使得输入的密码必须输入英文文本
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder sbRet = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    sbRet.AppendFormat("{0:X2}", b);
                }
                encodeStr = sbRet.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return encodeStr;
        }
        #endregion

        #region 对称解密方法
        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="pToDecrypt">要解密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>解密后的字符串</returns>
        public static string SymmetricDesDecode(string pToDecrypt, string sKey)
        {
            string decodeStr = string.Empty;
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                //建立加密对象的密钥和偏移量，此值重要，不能更改
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                //建立StringBuilder对象,CreateDecryptor使用的是流对象,必须把解密后的文本变成流对象
                StringBuilder sbRet = new StringBuilder();
                //return HttpContext.Current.Server.UrlDecode(System.Text.Encoding.Default.GetString(ms.ToArray()));
                decodeStr = System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return decodeStr;
        }
        #endregion

        #endregion
    }
}
