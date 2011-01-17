using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Baby.Framework.Comm
{
    /// <summary>
    /// 异步正则类
    /// </summary>
    public class BabyRegex
    {
        private int _timeOut = 0;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        private int _sleepInterval = 100;

        /// <summary>
        /// 时间计算步长
        /// </summary>
        public int SleepInterval
        {
            get { return _sleepInterval; }
            set { _sleepInterval = value; }
        }

        private bool _isComplete = false;

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsComplete 
        { 
            get { return _isComplete; } 
            set { _isComplete = value; } 
        }

        private MatchCollection _mc = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeOut"></param>
        public BabyRegex(int timeOut)
        {
            this._timeOut = timeOut;
        }
        
        /// <summary>
        /// 使用线程进行匹配，匹配成功则直接返回
        /// </summary>
        /// <param name="reg">正则</param>
        /// <param name="inputStr">需要匹配的字符串</param>
        /// <returns>匹配结果</returns>
        public MatchCollection Matchs(Regex reg,string inputStr)
        {
            Thread matchsThread = new Thread(new ThreadStart(delegate(){
                _mc = reg.Matches(inputStr);
                _isComplete = true;
            }));
            matchsThread.Start();
            int i = 0;
            while (_isComplete == false && matchsThread.IsAlive)
            {
                Thread.Sleep(SleepInterval);
                if ((++i) * SleepInterval >= _timeOut)
                {
                    matchsThread.Abort();
                    matchsThread = null;
                    _isComplete = true;
                }
            }

            return _mc;
        }

        /// <summary>
        /// 使用线程进行验证是否可以匹配，超时也认为匹配失败
        /// </summary>
        /// <param name="reg">正则</param>
        /// <param name="inputStr">需要匹配的字符串</param>
        /// <returns>true 成功；false 失败</returns>
        public bool IsMatch(Regex reg,string inputStr)
        {
            bool matchResult = false;
            Thread matchThread = new Thread(new ThreadStart(delegate(){
                matchResult = reg.IsMatch(inputStr);
                _isComplete = true;
            }));
            matchThread.Start();

            int i = 0;
            while (_isComplete == false && matchThread.IsAlive)
            {
                Thread.Sleep(SleepInterval);
                if ((++i) * SleepInterval >= _timeOut)
                {
                    matchThread.Abort();
                    matchThread = null;
                    _isComplete = true;
                }
            }

            return matchResult;
        }
    }

    /// <summary>
    /// 正则验证工具类
    /// </summary>
    internal static partial class BabyRegexTool
    {
        /// <summary>
        /// 获取字符串中第一张图片的地址
        /// </summary>
        /// <param name="str">带有图片地址的字符串</param>
        /// <returns>第一张图片的地址</returns>
        public static string GetFirstImgUrl(string str)
        {
            string regImg = @"<img.*?src\s*=\s*(""|')?(?<src>.*?)(""|')?(\s|>)";
            string srcUrl = string.Empty;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regImg, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Match m = regex.Match(str);
            if (m.Success)
            {
                srcUrl = m.Groups["src"].Value;
            }
            return srcUrl;
        }

        /// <summary>
        /// 是否为日期型字符串
        /// </summary>
        /// <param name="StrSource">日期字符串(2008-05-08)或者(2008-05)或者(2008)</param>
        /// <returns></returns>
        public static bool IsDate(string StrSource)
        {
            bool bdate = false;
            bdate = global:: System.Text.RegularExpressions.Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
            if (bdate == false) bdate = global:: System.Text.RegularExpressions.Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012]))|(((1[6-9]|[2-9]\d)\d{2})-0?2)|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
            if (bdate == false) bdate = global:: System.Text.RegularExpressions.Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2}))|(((1[6-9]|[2-9]\d)\d{2}))|(((1[6-9]|[2-9]\d)\d{2}))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
            return bdate;
        }

        /// <summary>
        /// 是否为时间型字符串
        /// </summary>
        /// <param name="source">时间字符串(15:00:00)</param>
        /// <returns></returns>
        public static bool IsTime(string StrSource)
        {
            return global:: System.Text.RegularExpressions.Regex.IsMatch(StrSource, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        /// 是否为日期+时间型字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(string StrSource)
        {
            return global:: System.Text.RegularExpressions.Regex.IsMatch(StrSource, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$ ");
        }

        /// <summary>
        /// 是否为网址
        /// </summary>
        /// <param name="path">网址字符串</param>
        /// <returns>true,是；false,否</returns>
        public static bool PathIsUrl(string path)
        {
            return global::System.Text.RegularExpressions.Regex.IsMatch(path, @"^(http://|ftp://|https://?.*)$");
        }
    }
}
